using System;
using Voxel.Noise.Util;

namespace Voxel.Noise.Modifiers
{
    public class Select : NoiseModule
    {
            /// Default edge-falloff value for the noise::module::Select noise module.
	    public const double DefaultSelectEdgeFalloff = 0.0;

	    /// Default lower bound of the selection range for the
	    /// noise::module::Select noise module.
	    public const double DefaultSelectLowerBound = -1.0;

	    /// Default upper bound of the selection range for the
	    /// noise::module::Select noise module.
	    public const double DefaultSelectUpperBound = 1.0;

	    /// Edge-falloff value.
        double edgeFalloff = DefaultSelectEdgeFalloff;

        public double EdgeFalloff
        {
            get { return edgeFalloff; }
            set 
            {
                    // Make sure that the edge falloff curves do not overlap.
                double boundSize = UpperBound - LowerBound;
                edgeFalloff = (value > boundSize / 2) ? boundSize / 2 : value;
            }
        }

	    /// Lower bound of the selection range.
	    public double LowerBound 
        {
            get;
            protected set;
        }

	    /// Upper bound of the selection range.
	    public double UpperBound 
        {
            get;
            protected set;
        }

        public NoiseModule ModuleA
        {
            get;
            set;
        }
        public NoiseModule ModuleB
        {
            get; 
            set;
        }

        public NoiseModule ControlModule
        {
            get;
            set;
        }



	    public Select()
        {
		    LowerBound = DefaultSelectLowerBound;
            UpperBound = DefaultSelectUpperBound;
	    }


	    public void SetBounds(double upper, double lower) 
        {
		    if (lower > upper)
			    throw new ArgumentException("lower must be less than upper");
		    LowerBound = lower;
		    UpperBound = upper;

		    EdgeFalloff = edgeFalloff;
	    }

	 
	    public override double GetValue(double x, double y, double z) {
            if (ModuleA == null)
                throw new InvalidOperationException("ModuleA cannot be null");
            if (ModuleB == null)
                throw new InvalidOperationException("ModuleB cannot be null");
            if (ControlModule == null)
                throw new InvalidOperationException("Control cannot be null");

            double controlValue = ControlModule.GetValue(x, y, z);
	        if (edgeFalloff > 0.0) 
            {
			    if (controlValue < (LowerBound - edgeFalloff)) 
                {
				    // The output value from the control module is below the selector
				    // threshold; return the output value from the first source module.
                    return ModuleA.GetValue(x, y, z);

			    }
                double alpha;
                if (controlValue < (LowerBound + edgeFalloff)) 
                {
                    // The output value from the control module is near the lower end of the
                    // selector threshold and within the smooth curve. Interpolate between
                    // the output values from the first and second source modules.
                    double lowerCurve = (LowerBound - edgeFalloff);
                    double upperCurve = (LowerBound + edgeFalloff);
                    alpha = NoiseMath.SCurve3((controlValue - lowerCurve) / (upperCurve - lowerCurve));
                    return NoiseMath.LinearInterpolate(ModuleA.GetValue(x, y, z), ModuleB.GetValue(x, y, z), alpha);

                }
                if (controlValue < (UpperBound - edgeFalloff)) 
                {
                    // The output value from the control module is within the selector
                    // threshold; return the output value from the second source module.
                    return ModuleB.GetValue(x, y, z);

                }
                if (controlValue < (UpperBound + edgeFalloff)) 
                {
                    // The output value from the control module is near the upper end of the
                    // selector threshold and within the smooth curve. Interpolate between
                    // the output values from the first and second source modules.
                    double lowerCurve = (UpperBound - edgeFalloff);
                    double upperCurve = (UpperBound + edgeFalloff);
                    alpha = NoiseMath.SCurve3((controlValue - lowerCurve) / (upperCurve - lowerCurve));
                    return NoiseMath.LinearInterpolate(ModuleB.GetValue(x, y, z), ModuleA.GetValue(x, y, z), alpha);

                }
                // Output value from the control module is above the selector threshold;
                // return the output value from the first source module.
                return ModuleA.GetValue(x, y, z);
            }
	        if (controlValue < LowerBound || controlValue > UpperBound) 
	        {
	            return ModuleA.GetValue(x, y, z);
	        }
	        return ModuleB.GetValue(x, y, z);
	    }
    }
}
