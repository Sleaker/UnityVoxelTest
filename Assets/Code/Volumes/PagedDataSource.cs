using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Voxel.MathUtil;

namespace Voxel.Volumes
{
    class PagedDataSource<T> : IVoxelDataSource<T>
    {
        private const int regionBits = 4;
        Dictionary<Vector3i, RegionData<T>> regionStore = new Dictionary<Vector3i, RegionData<T>>();

        private RegionData<T> GetOrCreateRegion(Vector3i location)
        {
            RegionData<T> DataCache = null;
            regionStore.TryGetValue(location, out DataCache);
            if (DataCache == null)
            {
                DataCache = new RegionData<T>(16, 1);
                regionStore.Add(location, DataCache);
            }
            return DataCache;
        }

        public void Set(float x, float y, float z, T data)
        {
            int xx = (int)x;
            int yy = (int)y;
            int zz = (int)z;

            Vector3i loc = new Vector3i(xx >> regionBits, yy >> regionBits, zz >> regionBits);
            RegionData<T> DataCache = GetOrCreateRegion(loc);

            DataCache.Set(VoxelUtil.WorldToChunkCoord(x), VoxelUtil.WorldToChunkCoord(y), VoxelUtil.WorldToChunkCoord(z), data);
        }

        public T Sample(float x, float y, float z, int level = 0)
        {
            int xx = (int)x;
            int yy = (int)y;
            int zz = (int)z;

            RegionData<T> DataCache = null;
            regionStore.TryGetValue(new Vector3i(xx >> regionBits, yy >> regionBits, zz >> regionBits), out DataCache);
            if (DataCache != null)
            {
                return DataCache.Sample(VoxelUtil.WorldToChunkCoord(x), VoxelUtil.WorldToChunkCoord(y), VoxelUtil.WorldToChunkCoord(z), level);
            }
            else
            {
                return default(T);
            }
        }

        public T[, ,] SampleSpace(Bounds Space)
        {
            int xvol = (int)Space.size.x;
            int yvol = (int)Space.size.y;
            int zvol = (int)Space.size.z;

            T[, ,] space = new T[xvol, yvol, zvol];

            int xoff = (int)Space.min.x;
            int yoff = (int)Space.min.y;
            int zoff = (int)Space.min.z;


            for (int x = 0; x < xvol; x++)
            {
                for (int y = 0; y < yvol; y++)
                {
                    for (int z = 0; z < zvol; z++)
                    {
                        T sample = Sample(xoff + x, yoff + y, zoff + z);
                        space[x, y, z] = sample;
                    }
                }

            }

            return space;
        }
    }
}
