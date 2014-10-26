using Voxel.Blocks;
using UnityEngine;
using Voxel.Volumes;
using Voxel.Noise.Generators;

namespace Voxel.Terrain.Generator
{
    class GroundGenerator : WorldGenerator<VoxelData>
    {
        private const int MaxHeight = 40;
        private const int MinHeight = 0;
        private const int HeightHalfDiff = (MaxHeight - MinHeight)/2;
        private static readonly Perlin Perlin = new Perlin();

        static GroundGenerator()
        {
            Perlin.Frequency = 0.01f;
            Perlin.Lacunarity = 2.0f;
            Perlin.Quality = Noise.NoiseQuality.High;
            Perlin.OctaveCount = 8;
            Perlin.Persistence = 0.5f;
        }

        public override void GenerateChunk(Bounds area, long seed, IVoxelDataSource<VoxelData> dataSource)
        {
            int xvol = (int)area.size.x;
            int yvol = (int)area.size.y;
            int zvol = (int)area.size.z;
           
            int xoff = (int)area.min.x;
            int yoff = (int)area.min.y;
            int zoff = (int)area.min.z;

            Block grass = Game.BlockRegistry["Grass"];
            Block dirt = Game.BlockRegistry["Dirt"];
            Block stone = Game.BlockRegistry["Stone"];

            for (int xx = 0; xx < xvol; xx++)
            {
                for (int zz = 0; zz < zvol; zz++)
                {
                    int landHeight = (int)System.Math.Floor(Perlin.GetValue(xx + xoff, 0, zz + zoff) * HeightHalfDiff + HeightHalfDiff + MinHeight);
                    int dirtHeight = (int)System.Math.Floor((Hash(xx + xoff, zz + zoff) >> 8 & 0xf) / 15f * 3) + 1;
                    for (int yy = 0; yy < yvol; yy++)
                    {
                        if (yoff == 0 && yy == 0)
                        {
                            // We don't want to fall through the world now do we?
                            dataSource.Set(xoff + xx, yoff + yy, zoff + zz, (VoxelData) stone.Id );
                        }
                        else if (yoff + yy > landHeight) continue;
                        if (yoff + yy == landHeight)
                        {
                            dataSource.Set(xoff + xx, yoff + yy, zoff + zz, (VoxelData) grass.Id ); 
                        }
                        else if (yoff + yy >= landHeight - dirtHeight)
                        {
                            dataSource.Set(xoff + xx, yoff + yy, zoff + zz, (VoxelData) dirt.Id );
                        } 
                        else
                        {
                            dataSource.Set(xoff + xx, yoff + yy, zoff + zz, (VoxelData) stone.Id );
                        } 
                    }
                }
            }
        }

        private static int Hash(int x, int y)
        {
            int hash = x * 3422543 ^ y * 432959;
            return hash * hash * (hash + 324319);
        }
    }
}
