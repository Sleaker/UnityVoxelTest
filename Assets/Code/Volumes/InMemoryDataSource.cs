using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Voxel.MathUtil;

namespace Voxel.Volumes
{


    public class RegionData<T>
    {
        Dictionary<int, T[, ,]> VoxelData = new Dictionary<int, T[, ,]>();


        int VoxelsPerUnit;
        int RegionSize = 16;

        public int MaxLod
        {
            get
            {
                return (int)Mathf.Log(RegionSize * VoxelsPerUnit, 2);
            }
        }

        public RegionData(int regionSize, int voxelsPerUnit)
        {
            /* if(!Mathf.IsPowerOfTwo(regionSize)) 
             {
                 Debug.Log("Tried to create a region with size not a power of 2.  Resizing to nearest power of 2");
                 regionSize = Mathf.NextPowerOfTwo(regionSize);
             } */


            RegionSize = regionSize;
            /*
            //If the # of voxels isn't a power of two, round up to the nearest power of 2
            if(!Mathf.IsPowerOfTwo(voxelsPerUnit))
            {
                voxelsPerUnit = Mathf.NextPowerOfTwo(voxelsPerUnit);
                Debug.Log("Tried to make voxels per unit a non-power of 2.  Resizing to nearest power of 2");
            }
             * */
            this.VoxelsPerUnit = voxelsPerUnit;

            int MaxLod = this.MaxLod;

            int voxelLength = RegionSize * voxelsPerUnit;
            int rsize = voxelLength;
            for (int i = 0; i <= MaxLod; i++)
            {
                VoxelData[i] = new T[rsize, rsize, rsize];
                rsize = rsize / (2);
            }
        }


        /// <summary>
        /// Returns voxel data from the given xyz, ranging from 0 to regionSize
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="level">LOD level to sample from</param>
        /// <returns></returns>
        public T Sample(float x, float y, float z, int level)
        {
            int xx, yy, zz;

            if (level > 0)
            {
                xx = (int)(x * VoxelsPerUnit) / (2 * (level + 1));
                yy = (int)(y * VoxelsPerUnit) / (2 * (level + 1));
                zz = (int)(z * VoxelsPerUnit) / (2 * (level + 1));
            }
            else
            {
                xx = (int)(x * VoxelsPerUnit);
                yy = (int)(y * VoxelsPerUnit);
                zz = (int)(z * VoxelsPerUnit);
            }

            return VoxelData[level][xx, yy, zz];
        }

        public void Set(float x, float y, float z, T data)
        {
            int xx = (int)(x * VoxelsPerUnit);
            int yy = (int)(y * VoxelsPerUnit);
            int zz = (int)(z * VoxelsPerUnit);

            VoxelData[0][xx, yy, zz] = data;

            //GenerateLODs(); //Possible Performance gain: Only generate lods for the voxels that this set operation effects
        }


        public void GenerateLODs()
        {
            for (int i = 0; i < MaxLod; i++)
            {
                int length = VoxelData[i].GetLength(0);
                for (int x = 0; x < length; x += 2)
                {
                    for (int y = 0; y < length; y += 2)
                    {
                        for (int z = 0; z < length; z += 2)
                        {
                            //Sample 8 voxels
                            T[] octant = {
                                    VoxelData[i][x, y, z],
                                    VoxelData[i][x+1, y, z],
                                    VoxelData[i][x, y+1, z],
                                    VoxelData[i][x , y, z+1],
                                    VoxelData[i][x+1, y+1, z],
                                    VoxelData[i][x+1, y , z+1],
                                    VoxelData[i][x, y+1, z+1],
                                    VoxelData[i][x+1, y+1, z+1],
                                };
                            //Find the cell that has the highest data
                            T max = octant.MostCommon(); //TODO get some kind of predicate here that works for
                            //any data type

                            VoxelData[i + 1][x / 2, y / 2, z / 2] = max;
                        }
                    }

                }
            }
        }

    }

    class InMemoryDataSource<T> : IVoxelDataSource<T>
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
