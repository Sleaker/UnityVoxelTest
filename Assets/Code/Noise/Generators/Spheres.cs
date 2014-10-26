using Voxel.Noise.Util;

namespace Voxel.Noise.Generators
{
    public class Spheres : NoiseModule
    {
        public float Frequency { get; set; }

        public Spheres()
        {
            Frequency = 1.0f;
        }

        public override double GetValue(double x, double y, double z)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            double distFromCenter = System.Math.Sqrt(x * x + y * y + z * z);
            int xInt = (x > 0.0 ? (int)x : (int)x - 1);
            double distFromSmallerSphere = distFromCenter - xInt;
            double distFromLargerSphere = 1.0 - distFromSmallerSphere;
            double nearestDist = NoiseMath.GetSmaller(distFromSmallerSphere, distFromLargerSphere);
            return 1.0 - (nearestDist * 4.0); // Puts it in the -1.0 to +1.0 range.
        }
    }
}
