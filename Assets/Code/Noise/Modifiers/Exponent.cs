using System;

namespace Voxel.Noise.Modifiers
{
    public class Exponent : NoiseModule
    {
        public const double DefaultExponent = 1.0;

        public double Exp
        {
            get;
            set;
        }

        public NoiseModule SourceModule
        {
            get;
            set;
        }
	    
	    public override double GetValue(double x, double y, double z) 
        {
		    if (SourceModule == null)
                throw new InvalidOperationException("Source Module cannot be null");

		    double value = SourceModule.GetValue(x, y, z);

		    return (Math.Pow(Math.Abs((value + 1.0) / 2.0), Exp) * 2.0 - 1.0);
	    }
    }
}