
namespace Voxel.MathUtil
{
    class VoxelUtil
    {
        public static float WorldToChunkCoord(float n)
        {
            int nn = (int)n;
            float frac = n - nn;
            return (nn & 0x0f) + frac;
        }
    }
}
