namespace Voxel.Noise.Generators
{
    public class Checkerboard : NoiseModule
    {
        public override double GetValue(double x, double y, double z)
        {
            int x0 = (x > 0.0 ? (int)x : (int)x - 1);
            int y0 = (y > 0.0 ? (int)y : (int)y - 1);
            int z0 = (z > 0.0 ? (int)z : (int)z - 1);

            int result = ((x0 & 1 ^ y0 & 1 ^ z0 & 1));
            if (result > 0) return -1.0;
            return 1.0;
        }
    }
}
