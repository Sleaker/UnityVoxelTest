using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxel.MathUtil;
using Voxel.Graphics;
using System.IO;

namespace Voxel.Blocks
{
    public class BlockRegistry
    {
        internal class RegisteredBlock
        {
            public Block Block;
        }

        private readonly Dictionary<string, RegisteredBlock> RegisteredBlocks = new Dictionary<string, RegisteredBlock>();

        private int BlockRegistrationId = 1;

        public Texture2D Atlas
        {
            get;
            private set;
        }

        internal Material AtlasMaterial
        {
            get;
            set;
        }

        Dictionary<AtlasSize, TextureAtlasLookup> TextureAtlases = new Dictionary<AtlasSize, TextureAtlasLookup>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The block, or null if it doesn't exist</returns>
        public Block this[string key]
        {
            get
            {
                return RegisteredBlocks.Where(kv => kv.Key == key).Select(kv => kv.Value.Block).FirstOrDefault();
            }
        }

        public Block this[int key]
        {
            get
            {
                return RegisteredBlocks.Where(kv => kv.Value.Block.Id == key).Select(kv => kv.Value.Block).FirstOrDefault();
            }
        }

        public int RegisteredBlockCount
        {
            get
            {
                return RegisteredBlocks.Count;
            }

        }


        public static void LoadBlockType(Block block)
        {
            BlockDataAttribute attrib = block.GetType().GetCustomAttributes(typeof(BlockDataAttribute), true).Cast<BlockDataAttribute>().FirstOrDefault();
            if (attrib == null)
            {
                // Skip blocks with invalid attributes
                Debug.LogError(block.GetType().Name + " must have a data attribute assigned to load properly.");
                return;
            }
            try
            {
                Game.BlockRegistry.RegisterBlock(block, attrib);
            }
            catch (InvalidOperationException e)
            {
                Debug.LogError(block.GetType().Name + " already loaded, so skipping . . .");
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The block, or null if it doesn't exist</returns>
        public Block GetBlock(string key)
        {
            return this[key];
        }

        private void RegisterBlock(Block b, BlockDataAttribute attributes)
        {
            b.Id = BlockRegistrationId;
            BlockRegistrationId++;

            b.Data = attributes;        

            Texture2D[] loadedTextures = new Texture2D[6];
            for(int i = 0; i < 6; i++)
            {
                Texture2D blockTexture = (Texture2D)Resources.Load(b.Data.TextureNames[i]);
                if (blockTexture == null)
                {
                    Debug.LogWarning("Block: " + b.Data.DisplayName + " references a texture that doesn't exist.  Falling back");
                    blockTexture = (Texture2D)Resources.Load("notexture");
                }

                loadedTextures[i] = blockTexture;
            }

                     

            string name = attributes.DisplayName;
            if (RegisteredBlocks.ContainsKey(name))
            {
                throw new InvalidOperationException("Cannot have two blocks with the same name!");
            }
            
            RegisteredBlocks[name] = new RegisteredBlock()
            {
                Block = b,
            };
        }

        public void RegistrationComplete()
        {
            IEnumerable<Block> blocks = RegisteredBlocks.Select(kv => kv.Value.Block);

            Texture2D noTexture = (Texture2D)Resources.Load("notexture");
            noTexture.name = "notexture";

            //Figure out what blocks will get what UV
            List<string> texturesToLoad = new List<string>();
            foreach(Block b in blocks)
            {
                foreach (string texture in b.Data.TextureNames)
                {
                    if (texture == "notexture") continue; //Skip 'notexture' textures, because are are already going to load
                                                          //those
                    if (!texturesToLoad.Contains(texture))
                    {
                        texturesToLoad.Add(texture);
                    }                                        
                }
            }

            //Load each of the textures, make sure they are square and a power of 2 between 32 and 512.
            List<Texture2D> LoadedTextures = new List<Texture2D>();
            List<string> NotLoadedTextures = new List<string>();
            LoadedTextures.Add(noTexture);
            foreach(string t in texturesToLoad)
            {
                if (t == "notexture") continue;
                Texture2D loadedTexture = (Texture2D)Resources.Load(t);
                loadedTexture.name = t;
                if (loadedTexture.width != loadedTexture.height)
                {
                    Debug.Log(string.Format("Texture {0} is not square!", t));
                    NotLoadedTextures.Add(t);
                    continue;
                }
                if (loadedTexture.width < 32 || loadedTexture.width > 512)
                {
                    Debug.Log(string.Format("Texture {0} must be a power of 2 between 32 and 512", t));
                    NotLoadedTextures.Add(t);
                    continue;
                }
                if (!Mathf.IsPowerOfTwo(loadedTexture.width))
                {
                    Debug.Log(string.Format("Texture {0} must be a power of 2 between 32 and 512", t));
                    NotLoadedTextures.Add(t);
                    continue;
                }

                LoadedTextures.Add(loadedTexture);
            }
                
            //Load and pack the textures
            for (int i = 32; i <= 512; i = i * 2)
            {
                IEnumerable<Texture2D> TexturesOfThisSize = LoadedTextures.Where(t => t.width == i);
                if (TexturesOfThisSize.Count() == 0) continue;

                //Pack them into an atlas
                Texture2D atlas = new Texture2D(1024, 1024, TextureFormat.ARGB32, true, false);
                atlas.anisoLevel = 9;
                atlas.mipMapBias = -.5f;
                atlas.filterMode = FilterMode.Point;
                float percent = 1f / 8f;
                float atlasWidth = atlas.PackTexturesWithTiling(TexturesOfThisSize.ToArray(), percent, 1024, false);
                atlas.Apply(true);
                
                AtlasSize thisSize = (AtlasSize)Enum.Parse(typeof(AtlasSize), "_" + i);
                Debug.Log("Building Atlas of size " + thisSize.ToString());
                            
                TextureAtlases[thisSize] = new TextureAtlasLookup()
                {
                    Atlas = atlas,
                    TextureNamesInThisAtlas = TexturesOfThisSize.Select(t => t.name).ToList(),
                    CopyPercent = percent,
                    PalleteSize = atlasWidth,
                };
            }
            
            //Go through the loaded names and assign IDs and Blocks to them.
            foreach(Block b in blocks)
            {
                List<int> Ids = new List<int>();
                List<AtlasSize> atlasLocations = new List<AtlasSize>();
                //Find which atlas each block's texture is in
                for(int i = 0; i < 6; i++)
                {
                    string textureName = b.Data.TextureNames[i];

                    //If we have a notexture or failed to load, index directly into 'notexture'
                    if(textureName == "notexture" || NotLoadedTextures.Contains(textureName))
                    {
                        Ids.Add(0); //notexture is the first texture in the 32sized array
                        atlasLocations.Add(AtlasSize._32);
                        continue;
                    }


                    //Find which atlas this texture is in
                    AtlasSize containingAtlas = AtlasSize._32;
                    foreach(KeyValuePair<AtlasSize, TextureAtlasLookup> kv in TextureAtlases)
                    {
                        if(kv.Value.TextureNamesInThisAtlas.Contains(textureName)) 
                        {
                            containingAtlas = kv.Key;
                            break;
                        }
                    }
                    atlasLocations.Add(containingAtlas);

                    //Find what position it's in
                    int index = TextureAtlases[containingAtlas].TextureNamesInThisAtlas.IndexOf(textureName);
                    Ids.Add(index);

                }

                b.Data.TextureIDs = Ids.ToArray();
                Debug.Log("block:" + b.Data.DisplayName);
                Debug.Log("TextureIds: " + string.Join(",", b.Data.TextureIDs.Select(x => x.ToString()).ToArray()));
                b.Data.AtlasLocations = atlasLocations.ToArray();
            }

            //Set up the material
            AtlasMaterial = (Material)Resources.Load("MCMaterial");
            AtlasMaterial.SetTexture("_Atlas32", TextureAtlases[AtlasSize._32].Atlas);
            AtlasMaterial.SetFloat("_PERCENT", TextureAtlases[AtlasSize._32].CopyPercent);
            AtlasMaterial.SetFloat("_PALLETESIZE", TextureAtlases[AtlasSize._32].PalleteSize);



            foreach(Block b in RegisteredBlocks.Values.Select(x => x.Block))
            {
                b.Data.RenderMaterial = AtlasMaterial;
            }

          
        }

    }

    internal enum AtlasSize
    {      
        _32,
        _64,
        _128,
        _256,
        _512,
    }

    internal class TextureAtlasLookup
    {
        public Texture2D Atlas;       
        public List<string> TextureNamesInThisAtlas;
        public float CopyPercent;
        public float PalleteSize;

    }







}
