using System;

namespace Voxel.Noise.Modifiers
{
    public class Multiply : NoiseModule
    {

        public NoiseModule Source
        {
            get;
            set;
        }
        public NoiseModule Source2
        {
            get;
            set;
        }
        public override double GetValue(double x, double y, double z)
        {
            if (Source == null)
                throw new InvalidOperationException("Source Module cannot be null");
            if (Source2 == null)
                throw new InvalidOperationException("Source Module cannot be null");

            return Source.GetValue(x, y, z) * Source2.GetValue(x, y, z);
        }
    }
}