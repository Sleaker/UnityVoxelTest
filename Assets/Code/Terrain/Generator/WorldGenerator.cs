using UnityEngine;
using Voxel.Volumes;

namespace Voxel.Terrain.Generator
{
    public abstract class WorldGenerator<T>
    {
        public abstract void GenerateChunk(Bounds area, long seed, IVoxelDataSource<T> dataSource);       
    }
}
