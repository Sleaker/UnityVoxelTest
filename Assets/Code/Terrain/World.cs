using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Voxel.Behavior;
using Voxel.Volumes;
using Voxel.Graphics;
using ThreadDispatcher;
using Voxel.MathUtil;
using Voxel.Blocks;
using Voxel.Terrain.Generator;

namespace Voxel.Terrain
{
    [ExecuteInEditMode]
    public class World : ModBehavior
    {
        public WorldGenerator<VoxelData> WorldGenerator;

        readonly Dictionary<Vector3, Chunk> LoadedChunks = new Dictionary<Vector3, Chunk>();

        IVoxelDataSource<VoxelData> dataSource;

        private IMesher mesher = new MCMesher();
        public IMesher Mesher {
            get { return mesher; }
            set
            {
                mesher = value;
                foreach (Chunk c in LoadedChunks.Values)
                {
                    c.Mesher = value;
                }
            } 
        }

        public bool EditMode { get; set; }

        public long Seed
        {
            get;
            set;
        }

        public void Awake()
        {
            this.tag = "World";
            dataSource = new InMemoryDataSource<VoxelData>();
            Debug.Log("Awake called on world, datasource set to: " + dataSource.ToString());
        }

        public void Update()
        {
            //Get all observers
            GameObject[] observers = GameObject.FindGameObjectsWithTag("Observer");
            foreach (Observer o in observers.Select(obs => obs.GetComponent<Observer>()).Where(o => o == null))
            {
                //If the object isn't actually an observer, ignore it but log a warning
                Debug.LogWarning("Object was tagged as observer but didn't have the observer component:" + o.name);                
            }
        }

        public void GenerateWorld()
        {
            //Load an area for testing
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        // TODO: use a Chunksize reference instead of arbitrarily setting to 16
                        int chunkX = x * 16;
                        int chunkY = y * 16;
                        int chunkZ = z * 16;

                        GameObject o = new GameObject(string.Format("{0}-chunk({1}, {2}, {3})", gameObject.name, x, y, z), typeof(Chunk));
                        Chunk c = o.GetComponent<Chunk>();
                        c.EditMode = EditMode;
                        c.transform.parent = gameObject.transform;
                        c.transform.localPosition = new Vector3(chunkX, chunkY, chunkZ);
                        c.dataSource = dataSource;

                        //Distribute the load
                        //TODO: make this more concurrent, threadsafe, and not lag the game when loading

                        // Bounds object is passed into the chunk mesher.
                        Bounds b = new Bounds();
                        b.SetMinMax(new Vector3(chunkX, chunkY, chunkZ), new Vector3(chunkX + 16, chunkY + 16, chunkZ + 16));

                        c.bounds = b;
                        if (!EditMode)
                        {
                            WorldGenerator.GenerateChunk(b, Seed, dataSource);
                            c.Mesher = mesher;
                            UnityThreadHelper.TaskDistributor.Dispatch(() =>
                            {
                                WorldGenerator.GenerateChunk(b, Seed, dataSource);

                                UnityThreadHelper.Dispatcher.Dispatch(() =>
                                {
                                    c.Mesher = mesher;
                                });
                            });
                        }
                        else
                        {
                            WorldGenerator.GenerateChunk(b, Seed, dataSource);
                            c.Mesher = mesher;
                        }
                        
                        LoadedChunks[new Vector3i(chunkX, chunkY, chunkZ)] = c;
                    }
                }
            }
        }


        public Block GetBlockAt(Vector3 position)
        {
            return Game.BlockRegistry[dataSource.Sample(position.x, position.y, position.z)];
        }

        public void ChangeBlock(Vector3 position, Block block)
        {
            int id = block == null ? 0 : block.Id;
            //If we are breaking a block, trigger it's on break event
            if(block == null)
            {
                Block previousBlock = GetBlockAt(position);
                previousBlock.Destroyed(position);
            }


            //Set the block in the data source
            Debug.Log("Placing: " + position.ToString());
            dataSource.Set(position.x, position.y, position.z, new VoxelData() { Material = (byte) id });

            //If the block isn't null, notify that it's been placed
            if(block != null)
            {
                block.Placed(position);
            }



            //Notify the chunk that it's data source has changed and it should probably remesh
            Chunk c = LoadedChunks.Values.SingleOrDefault(x => x.bounds.Contains(position));

            if (c != null)
            {                
                //rebuild neighor chunks if we are on the border
                int xx = (int)position.x / 16;
                int yy = (int)position.y / 16;
                int zz = (int)position.z / 16;


                IEnumerable<Chunk> neighbors = LoadedChunks.Values.Where(x =>
                    {
                        int cx = (int)x.bounds.min.x / 16;
                        int cy = (int)x.bounds.min.y / 16;
                        int cz = (int)x.bounds.min.z / 16;

                        return Vector3.Distance(new Vector3(xx, yy, zz), new Vector3(cx, cy, cz)) <= 1;
                    });
                foreach(Chunk cnk in neighbors)
                {
                    cnk.BuildChunk();
                }

                c.BuildChunk();
            }
            else
            {
                Debug.Log("Chunk Null!");
            }
        }        
    }
}
