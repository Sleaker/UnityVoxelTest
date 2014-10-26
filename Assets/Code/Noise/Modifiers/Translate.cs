using System;

namespace Voxel.Noise.Modifiers
{
    public class Translate : NoiseModule
    {

          /// Default translation factor applied to the @a x coordinate for the
	    /// noise::module::TranslatePoint noise module.
	    public const double DefaultTranslatePointX = 0.0;

	    /// Default translation factor applied to the @a y coordinate for the
	    /// noise::module::TranslatePoint noise module.
	    public const double DefaultTranslatePointY = 0.0;

	    /// Default translation factor applied to the @a z coordinate for the
	    /// noise::module::TranslatePoint noise module.
	    public const double DefaultTranslatePointZ = 0.0;

	    /// Translation amount applied to the @a x coordinate of the input
	    /// value.
	    double XTranslation 
        {
            get;
            set;
        }

	    /// Translation amount applied to the @a y coordinate of the input
	    /// value.
	    double YTranslation 
        {
            get; 
            set;
        }

	    /// Translation amount applied to the @a z coordinate of the input
	    /// value.
	    double ZTranslation 
        {
            get; 
            set;
        }


        public NoiseModule SourceModule
        {
            get;
            set;
        }

	    public Translate() {
		    XTranslation = DefaultTranslatePointX;
            YTranslation = DefaultTranslatePointY;
            ZTranslation = DefaultTranslatePointZ;
	    }	    

	    public void SetTranslations(double x, double y, double z) {
		    XTranslation = x;
		    YTranslation = y;
		    ZTranslation = z;
	    }

	    public override double GetValue(double x, double y, double z) {
            if (SourceModule == null)
                throw new InvalidOperationException("Source cannot be null");

		    return SourceModule.GetValue(x + XTranslation, y + YTranslation, z + ZTranslation);
	    }
    }
}
