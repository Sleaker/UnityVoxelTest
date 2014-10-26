using Voxel.Noise.Util;

namespace Voxel.Noise.Generators
{
    public class Voronoi : NoiseModule
    {
        public double Frequency = 1.0;
        public double Displacement = 1.0;
        public bool DistanceEnabled = false;

        public Voronoi()
        {
            Frequency = 1.0;
            Displacement = 1.0;
            Seed = 0;
            DistanceEnabled = false;
        }

        public override double GetValue(double x, double y, double z)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            int xInt = (x > 0.0 ? (int)x : (int)x - 1);
            int yInt = (y > 0.0 ? (int)y : (int)y - 1);
            int zInt = (z > 0.0 ? (int)z : (int)z - 1);

            double minDist = 2147483647.0;
            double xCandidate = 0;
            double yCandidate = 0;
            double zCandidate = 0;

            // Inside each unit cube, there is a seed point at a random position.  Go
            // through each of the nearby cubes until we find a cube with a seed point
            // that is closest to the specified position.
            for (int zCur = zInt - 2; zCur <= zInt + 2; zCur++)
            {
                for (int yCur = yInt - 2; yCur <= yInt + 2; yCur++)
                {
                    for (int xCur = xInt - 2; xCur <= xInt + 2; xCur++)
                    {

                        // Calculate the position and distance to the seed point inside of
                        // this unit cube.
                        double xPos = xCur + GeneratorBase.ValueNoise(xCur, yCur, zCur, Seed);
                        double yPos = yCur + GeneratorBase.ValueNoise(xCur, yCur, zCur, Seed + 1);
                        double zPos = zCur + GeneratorBase.ValueNoise(xCur, yCur, zCur, Seed + 2);
                        double xDist = xPos - x;
                        double yDist = yPos - y;
                        double zDist = zPos - z;
                        double dist = xDist * xDist + yDist * yDist + zDist * zDist;

                        if (!(dist < minDist)) continue;
                        // This seed point is closer to any others found so far, so record
                        // this seed point.
                        minDist = dist;
                        xCandidate = xPos;
                        yCandidate = yPos;
                        zCandidate = zPos;
                    }
                }
            }

            double value;
            if (DistanceEnabled)
            {
                // Determine the distance to the nearest seed point.
                double xDist = xCandidate - x;
                double yDist = yCandidate - y;
                double zDist = zCandidate - z;
                value = (System.Math.Sqrt(xDist * xDist + yDist * yDist + zDist * zDist)
                    ) * NoiseMath.Sqrt3 - 1.0;
            }
            else
            {
                value = 0.0;
            }

            int x0 = (xCandidate > 0.0 ? (int)xCandidate : (int)xCandidate - 1);
            int y0 = (yCandidate > 0.0 ? (int)yCandidate : (int)yCandidate - 1);
            int z0 = (zCandidate > 0.0 ? (int)zCandidate : (int)zCandidate - 1);

            // Return the calculated distance with the displacement value applied.
            return value + (Displacement * GeneratorBase.ValueNoise(x0, y0, z0));
        }
    }
}
