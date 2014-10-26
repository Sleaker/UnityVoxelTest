using Voxel.Noise.Util;

namespace Voxel.Noise.Generators
{
    public class Cylinders : NoiseModule
    {
        public double Frequency = 1.0;
        public override double GetValue(double x, double y, double z)
        {
            x *= Frequency;
            z *= Frequency;

            double distFromCenter = System.Math.Sqrt(x * x + z * z);
            int distFromCenter0 = (distFromCenter > 0.0 ? (int)distFromCenter : (int)distFromCenter - 1);
            double distFromSmallerSphere = distFromCenter - distFromCenter0;
            double distFromLargerSphere = 1.0 - distFromSmallerSphere;
            double nearestDist = NoiseMath.GetSmaller(distFromSmallerSphere, distFromLargerSphere);
            return 1.0 - (nearestDist * 4.0);
        }
    }
}
