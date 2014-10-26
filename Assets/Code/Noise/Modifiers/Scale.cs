using System;

namespace Voxel.Noise.Modifiers
{
    public class Scale : NoiseModule
    {
        /// Default scaling factor applied to the @a x coordinate for the
	    /// noise::module::ScalePoint noise module.
	    public const double DefaultScalePointX = 1.0;

	    /// Default scaling factor applied to the @a y coordinate for the
	    /// noise::module::ScalePoint noise module.
	    public const double DefaultScalePointY = 1.0;

	    /// Default scaling factor applied to the @a z coordinate for the
	    /// noise::module::ScalePoint noise module.
	    public const double DefaultScalePointZ = 1.0;

	    /// Scaling factor applied to the @a x coordinate of the input value.
	    public double XScale
        {
            get;
            set;
        }

	    /// Scaling factor applied to the @a y coordinate of the input value.
	    public double YScale 
        {
            get;
            set;
        }

	    /// Scaling factor applied to the @a z coordinate of the input value.
	    public double ZScale
        {
            get;
            set;
        }

        public NoiseModule SourceModule
        {
            get;
            set;
        }

	    public Scale() 
        {
		    XScale = DefaultScalePointX;
            YScale = DefaultScalePointY;
            ZScale = DefaultScalePointZ;
	    }
	    
	    public override double GetValue(double x, double y, double z) 
        {
		    if (SourceModule == null)
                throw new InvalidOperationException("Source cannot be null");

		    return SourceModule.GetValue(x * XScale, y * YScale, z * ZScale);
	    }
    }
}
