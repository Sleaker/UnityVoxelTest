using System;
using Voxel.Noise.Generators;

namespace Voxel.Noise.Modifiers
{
    public class Turbulence : NoiseModule
    {
        /// Default frequency for the noise::module::Turbulence noise module.
	    public const double DefaultTurbulenceFrequency = Perlin.DefaultPerlinFrequency;

	    /// Default power for the noise::module::Turbulence noise module.
	    public const double DefaultTurbulencePower = 1.0;

	    /// Default roughness for the noise::module::Turbulence noise module.
	    public const int DefaultTurbulenceRoughness = 3;

	    /// Default noise seed for the noise::module::Turbulence noise module.
	    public const int DefaultTurbulenceSeed = Perlin.DefaultPerlinSeed;

	    /// The power (scale) of the displacement.
	    public double Power
        {
            get;
            set;
        }

        public int Roughness
        {
            get 
            {
                return xDistortModule.OctaveCount;
            }
            set
            {
                xDistortModule.OctaveCount = (value);
		        yDistortModule.OctaveCount = (value);
		        zDistortModule.OctaveCount = (value);
            }

        }

        public new int Seed
        {
            get
            {
                return xDistortModule.Seed;
            }
            set
            {
                xDistortModule.Seed = value;
		        yDistortModule.Seed = (value + 1);
		        zDistortModule.Seed = (value + 2);
            }
        }

        public double Frequency
        {
            get 
            {
                return xDistortModule.Frequency;
            }
            set
            {
                xDistortModule.Frequency = (value);
		        yDistortModule.Frequency = (value);
		        zDistortModule.Frequency = (value);
            }
        }
        public NoiseModule SourceModule
        {
            get;
            set;
        }

	    /// Noise module that displaces the @a x coordinate.
	    readonly Perlin xDistortModule;

	    /// Noise module that displaces the @a y coordinate.
	    readonly Perlin yDistortModule;

	    /// Noise module that displaces the @a z coordinate.
	    readonly Perlin zDistortModule;

	    public Turbulence() {		   
		    xDistortModule = new Perlin();
		    yDistortModule = new Perlin();
		    zDistortModule = new Perlin();

            Power = DefaultTurbulencePower;
	    }	   


	    public override double GetValue(double x, double y, double z) {
            if (SourceModule == null)
                throw new InvalidOperationException("Source cannot be null");

		    // Get the values from the three noise::module::Perlin noise modules and
		    // add each value to each coordinate of the input value.  There are also
		    // some offsets added to the coordinates of the input values.  This prevents
		    // the distortion modules from returning zero if the (x, y, z) coordinates,
		    // when multiplied by the frequency, are near an integer boundary.  This is
		    // due to a property of gradient coherent noise, which returns zero at
		    // integer boundaries.
		    double x0, y0, z0;
		    double x1, y1, z1;
		    double x2, y2, z2;
		    x0 = x + (12414.0 / 65536.0);
		    y0 = y + (65124.0 / 65536.0);
		    z0 = z + (31337.0 / 65536.0);
		    x1 = x + (26519.0 / 65536.0);
		    y1 = y + (18128.0 / 65536.0);
		    z1 = z + (60493.0 / 65536.0);
		    x2 = x + (53820.0 / 65536.0);
		    y2 = y + (11213.0 / 65536.0);
		    z2 = z + (44845.0 / 65536.0);
		    double xDistort = x + (xDistortModule.GetValue(x0, y0, z0) * Power);
		    double yDistort = y + (yDistortModule.GetValue(x1, y1, z1) * Power);
		    double zDistort = z + (zDistortModule.GetValue(x2, y2, z2) * Power);

		    // Retrieve the output value at the offsetted input value instead of the
		    // original input value.
		    return SourceModule.GetValue(xDistort, yDistort, zDistort);

	    }

    }
}
