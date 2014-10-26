using System;

namespace Voxel.Noise.Modifiers
{
    public class Invert : NoiseModule
    {
        public NoiseModule SourceModule
        {
            get;
            set;
        }

        public override double GetValue(double x, double y, double z)
        {
            if (SourceModule == null)
                throw new InvalidOperationException("Source Module cannot be null");

            return -(SourceModule.GetValue(x, y, z));
        }
    }
}
