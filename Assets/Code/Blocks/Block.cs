using System;
using UnityEngine;

namespace Voxel.Blocks
{
    [Serializable]
    public class BlockDataAttribute : Attribute
    {
        public readonly string DisplayName;
        public readonly string[] TextureNames;
        public readonly int Hardness;
        public readonly bool Opaque;

        internal AtlasSize[] AtlasLocations
        {
            get;
            set;
        }

        internal int[] TextureIDs
        {
            get;
            set;
        }

        public Material RenderMaterial
        {
            get;
            internal set;
        }


        public BlockDataAttribute(string displayName) :
            this(displayName, "NoTexture")
        {
            
        }

        public BlockDataAttribute(string displayName, string[] textures)
            : this(displayName, textures, 1)
        
        {

        }

        public BlockDataAttribute(string displayName, string[] textures, int hardness)
            : this(displayName, textures, hardness, true)
        {

        }

        

        public BlockDataAttribute(string displayName, string textureName) :
            this(displayName, textureName, 1)
        {

        }

        public BlockDataAttribute(string displayName, string textureName, int hardness)
            : this(displayName, textureName, hardness, true)
        {

        }

        public BlockDataAttribute(string displayName, string textureName, int hardness, bool opaque)
            : this(displayName, new string[] { textureName, textureName, textureName, textureName, textureName, textureName }, hardness, opaque)
        {
            
        }

        public BlockDataAttribute(string displayName, string[] textures, int hardness, bool opaque)
        {
            if (textures.Length != 6) throw new ArgumentException("Must provide 6 textures (one for each face of the cube)");
            DisplayName = displayName;
            TextureNames = textures;
            Hardness = hardness;
            Opaque = opaque;
        }
    }

    [Serializable]
    public abstract class Block
    {
        public int Id
        {
            get;
            internal set;
        }
        public BlockDataAttribute Data
        {
            get;
            internal set;
        }
     
        public abstract void Placed(Vector3 worldLocation);
        public abstract void Destroyed(Vector3 worldLocation);
    }

    // Actual block implementations


    [BlockData("Grass", new[] { "Blocks/grass_side", "Blocks/grass_side", "Blocks/wool_colored_green", "Blocks/dirt", "Blocks/grass_side", "Blocks/grass_side" })]
    class Grass : SimpleBlock
    {
    }

    [BlockData("Dirt", "Blocks/dirt")]
    class Dirt : SimpleBlock
    {
    }

    [BlockData("Cobblestone", "Blocks/cobblestone")]
    class Cobblestone : SimpleBlock
    {
    }

    [BlockData("Mossy Cobblestone", "Blocks/cobblestone_mossy")]
    class MossyCobblestone : SimpleBlock
    {
    }

    [BlockData("Stone", "Blocks/stone")]
    class Stone : SimpleBlock
    {
    }
}
