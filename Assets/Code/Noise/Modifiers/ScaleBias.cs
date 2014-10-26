using System;

namespace Voxel.Noise.Modifiers
{
    public class ScaleBias : NoiseModule
    {
    /// Default bias for the noise::module::ScaleBias noise module.
	    public const double DefaultBias = 0.0;

	    /// Default scale for the noise::module::ScaleBias noise module.
	    public const double DefaultScale = 1.0;

	    /// Bias to apply to the scaled output value from the source module.
        public double Bias
        {
            get;
            set;
        }

	    /// Scaling factor to apply to the output value from the source
	    /// module.
        public double Scale
        {
            get;
            set;
        }

        public NoiseModule SourceModule
        {
            get;
            set;
        }


	    public ScaleBias() 
        {
            Bias = DefaultBias;
            Scale = DefaultScale;
	    }
	   
	    public override double GetValue(double x, double y, double z) 
        {
            if (SourceModule == null)
                throw new InvalidOperationException("Source cannot be null");

		    return SourceModule.GetValue(x, y, z) * Scale + Bias;
	    }

    }
}