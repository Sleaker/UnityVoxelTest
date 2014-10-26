using System;

namespace Voxel.Noise.Modifiers
{
    public class Power : NoiseModule
    {
        public NoiseModule Base
        {
            get;
            set;
        }
        public NoiseModule Exponent
        {
            get;
            set;
        }
        public override double GetValue(double x, double y, double z)
        {
            if (Base == null)
                throw new InvalidOperationException("Base cannot be null");
            if (Exponent == null)
                throw new InvalidOperationException("Exponent cannot be null");

            return Math.Pow(Base.GetValue(x, y, z), Exponent.GetValue(x, y, z));
        }
    }
}
