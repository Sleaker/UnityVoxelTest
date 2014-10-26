using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace Voxel.Volumes
{
    [StructLayout(LayoutKind.Explicit, Size = 3)]
    public struct VoxelData
    {
        [FieldOffset(0)]
        public uint Data;

        [FieldOffset(0)]
        public byte Material;

        [FieldOffset(1)]
        public sbyte Distance;

        [FieldOffset(2)]
        public byte Sharpness;

        // Implicit type conversion to unsigned short, for easily parsing into the Material definition.
        public static implicit operator ushort(VoxelData d)
        {
            return d.Material;
        }

        public static explicit operator uint(VoxelData d)
        {
            return d.Data;
        }

        public static explicit operator VoxelData(uint i)
        {
            return new VoxelData() { Data = i };
        }

        public static explicit operator VoxelData(int i)
        {
            return new VoxelData() { Data = (uint)i };
        }
    }

    public interface IVoxelDataSource<T>
    {

        /// <summary>
        /// Sets the data at a given x,y,z.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="data"></param>
        void Set(float x, float y, float z, T data);


        /// <summary>
        /// Function to grab the voxel data at a certain point.
        /// 
        /// Any non-zero value is considered solid
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns>0 for no voxel, non zero for solid</returns>
        T Sample(float x, float y, float z, int level = 0);

        /// <summary>
        /// Samples a 3d volume of voxels
        /// </summary>
        /// <param name="space"></param>
        /// <returns></returns>
        T[,,] SampleSpace(Bounds space);
    }

    public enum VolumeType
    {
        Block,
        MarchingCubes
        //Transvoxel
    }
}
