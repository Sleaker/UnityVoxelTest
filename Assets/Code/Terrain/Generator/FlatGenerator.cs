using Voxel.Blocks;
using UnityEngine;
using System;
using Voxel.Volumes;
using Voxel.Exceptions;

namespace Voxel.Terrain.Generator
{
    public class FlatGenerator : WorldGenerator<VoxelData>
    {
        public Block ToGenerate { get; internal set; }
        public int Height { get; internal set; }

        public FlatGenerator(string block, int height)
        {
            var found = Game.BlockRegistry[block];
            if (found == null)
            {
                throw new BlockNotFoundException(block);
            }
            if (height < 1)
            {
                throw new ArgumentOutOfRangeException("height", "Height must be greater than 0!");
            }
            ToGenerate = found;
            Height = height;
        }

        public override void GenerateChunk(Bounds area, long seed, IVoxelDataSource<VoxelData> dataSource)
        {
            var xvol = (int)area.size.x;
            var zvol = (int)area.size.z;

            var xoff = (int)area.min.x;
            var zoff = (int)area.min.z;

            for (var xx = 0; xx < xvol; xx++)
            {
                for (var yy = 0; yy < zvol; yy++)
                {
                    for (var d = 0; d <= Height; d++)
                    {
                        dataSource.Set(xx + xoff, d, yy + zoff, new VoxelData() { Material = (byte) ToGenerate.Id });
                    }
                }
            }            
        }
    }
}
