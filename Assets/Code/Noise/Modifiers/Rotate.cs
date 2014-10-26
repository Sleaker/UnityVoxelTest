using System;
using Voxel.Noise.Util;

namespace Voxel.Noise.Modifiers
{
    public class Rotate : NoiseModule
    {
           /// Default @a x rotation angle for the noise::module::RotatePoint noise
	    /// module.
	    public const double DefaultRotateX = 0.0;

	    /// Default @a y rotation angle for the noise::module::RotatePoint noise
	    /// module.
	    public const double DefaultRotateY = 0.0;

	    /// Default @a z rotation angle for the noise::module::RotatePoint noise
	    /// module.
	    public const double DefaultRotateZ = 0.0;


        public NoiseModule SourceModule
        {
            get;
            set;
        }


	    double xAngle = DefaultRotateX;
	    double yAngle = DefaultRotateY;
	    double zAngle = DefaultRotateZ;

        public double XAngle
        {
            get { return xAngle; }
            set { setAngles(value, yAngle, zAngle); }

        }

        public double YAngle
        {
            get { return yAngle; }
            set { setAngles(xAngle, value, zAngle); }
        }

        public double ZAngle
        {
            get { return zAngle; }
            set { setAngles(xAngle, yAngle, value); }
        }



	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double x1Matrix;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double x2Matrix;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double x3Matrix;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double y1Matrix;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double y2Matrix;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double y3Matrix;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double z1Matrix;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double z2Matrix;

	    /// An entry within the 3x3 rotation matrix used for rotating the
	    /// input value.
	    double z3Matrix;

	    public Rotate() {
		
		    setAngles(DefaultRotateX, DefaultRotateY, DefaultRotateZ);
	    }

	    public void setAngles(double x, double y, double z) {
	        double xCos = Math.Cos(x * NoiseMath.DegToRad);
            double yCos = Math.Cos(y * NoiseMath.DegToRad);
            double zCos = Math.Cos(z * NoiseMath.DegToRad);
            double xSin = Math.Sin(x * NoiseMath.DegToRad);
            double ySin = Math.Sin(y * NoiseMath.DegToRad);
            double zSin = Math.Sin(z * NoiseMath.DegToRad);

		    x1Matrix = ySin * xSin * zSin + yCos * zCos;
		    y1Matrix = xCos * zSin;
		    z1Matrix = ySin * zCos - yCos * xSin * zSin;
		    x2Matrix = ySin * xSin * zCos - yCos * zSin;
		    y2Matrix = xCos * zCos;
		    z2Matrix = -yCos * xSin * zCos - ySin * zSin;
		    x3Matrix = -ySin * xCos;
		    y3Matrix = xSin;
		    z3Matrix = yCos * xCos;

		    xAngle = x;
		    yAngle = y;
		    zAngle = z;

	    }


	    public override double GetValue(double x, double y, double z) {
		    if (SourceModule == null)
                throw new InvalidOperationException("Source Module cannot be null");

		    double nx = (x1Matrix * x) + (y1Matrix * y) + (z1Matrix * z);
		    double ny = (x2Matrix * x) + (y2Matrix * y) + (z2Matrix * z);
		    double nz = (x3Matrix * x) + (y3Matrix * y) + (z3Matrix * z);
		    return SourceModule.GetValue(nx, ny, nz);
	    }
    }
}
