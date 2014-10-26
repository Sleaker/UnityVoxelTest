using System;

namespace Voxel.Noise.Modifiers
{
    public class Clamp : NoiseModule
    {
        public double LowerBound = 0;
        public double UpperBound = 0;

        public NoiseModule SourceModule;


        public override double GetValue(double x, double y, double z)
        {
            if (SourceModule == null)
                throw new NullReferenceException("A source module must be provided.");

            if (LowerBound >= UpperBound)
                throw new Exception("Lower bound must be lower than upper bound.");

            double value = SourceModule.GetValue(x, y, z);
            if (value < LowerBound)
            {
                return LowerBound;
            }
            return value > UpperBound ? UpperBound : value;
        }
    }
}
