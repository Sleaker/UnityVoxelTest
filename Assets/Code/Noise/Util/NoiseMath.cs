namespace Voxel.Noise.Util
{
	
    internal static class NoiseMath
    {
        /// <summary>
        /// Returns the given value clamped between the given lower and upper bounds.
        /// </summary>
        internal static int ClampValue(int value, int lowerBound, int upperBound)
        {
            if (value < lowerBound)
            {
                return lowerBound;
            }
            return value > upperBound ? upperBound : value;
        }

        /// <summary>
        /// Returns the cubic interpolation of two values bound between two other values.
        /// </summary>
        /// <param name="n0">The value before the first value.</param>
        /// <param name="n1">The first value.</param>
        /// <param name="n2">The second value.</param>
        /// <param name="n3">The value after the second value.</param>
        /// <param name="a">The alpha value.</param>
        /// <returns></returns>
        internal static double CubicInterpolate(double n0, double n1, double n2, double n3, double a)
        {
            double p = (n3 - n2) - (n0 - n1);
            double q = (n0 - n1) - p;
            double r = n2 - n0;
            double s = n1;
            return p * a * a * a + q * a * a + r * a + s;
        }

        /// <summary>
        /// Returns the smaller of the two given numbers.
        /// </summary>
        internal static double GetSmaller(double a, double b)
        {
            return (a < b ? a : b);
        }

        /// <summary>
        /// Returns the larger of the two given numbers.
        /// </summary>
        internal static double GetLarger(double a, double b)
        {
            return (a > b ? a : b);
        }

        /// <summary>
        /// Swaps the values contained by the two given variables.
        /// </summary>
        internal static void SwapValues(ref double a, ref double b)
        {
            double c = a;
            a = b;
            b = c;
        }

        /// <summary>
        /// Returns the linear interpolation of two values with the given alpha.
        /// </summary>
        internal static double LinearInterpolate(double n0, double n1, double a)
        {
            return ((1.0 - a) * n0) + (a * n1);
        }

        /// <summary>
        /// Returns the given value, modified to be able to fit into a 32-bit integer.
        /// </summary>
        internal static double MakeInt32Range(double n)
        {
            if (n >= 1073741824.0)
            {
                return ((2.0 * System.Math.IEEERemainder(n, 1073741824.0)) - 1073741824.0);
            }
            else if (n <= -1073741824.0)
            {
                return ((2.0 * System.Math.IEEERemainder(n, 1073741824.0)) + 1073741824.0);
            }
            else
            {
                return n;
            }
        }

        /// <summary>
        /// Returns the given value mapped onto a cubic S-curve.
        /// </summary>
        internal static double SCurve3(double a)
        {
            return (a * a * (3.0 - 2.0 * a));
        }

        /// <summary>
        /// Returns the given value mapped onto a quintic S-curve.
        /// </summary>
        internal static double SCurve5(double a)
        {
            double a3 = a * a * a;
            double a4 = a3 * a;
            double a5 = a4 * a;
            return (6.0 * a5) - (15.0 * a4) + (10.0 * a3);
        }

        /// <summary>
        /// Returns the value of the mathematical constant PI.
        /// </summary>
        internal static readonly double PI = 3.1415926535897932385;

        /// <summary>
        /// Returns the square root of 2.
        /// </summary>
        internal static readonly double Sqrt2 = 1.4142135623730950488;

        /// <summary>
        /// Returns the square root of 3.
        /// </summary>
        internal static readonly double Sqrt3 = 1.7320508075688772935;

        /// <summary>
        /// Returns PI/180.0, used for converting degrees to radians.
        /// </summary>
        internal static readonly double DegToRad = PI / 180.0;

        /// <summary>
        /// Provides the X, Y, and Z coordinates on the surface of a sphere 
        /// cooresponding to the given latitude and longitude.
        /// </summary>
        internal static void LatLonToXYZ(double lat, double lon, ref double x, ref double y, ref double z)
        {
            double r = System.Math.Cos(DegToRad * lat);
            x = r * System.Math.Cos(DegToRad * lon);
            y = System.Math.Sin(DegToRad * lat);
            z = r * System.Math.Sin(DegToRad * lon);
        }

    }

}
