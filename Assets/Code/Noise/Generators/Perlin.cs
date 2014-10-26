using System;
using Voxel.Noise.Util;

namespace Voxel.Noise.Generators
{
    public class Perlin : NoiseModule
    {

        /// Default frequency for the noise::module::Perlin noise module.
	    public const double DefaultPerlinFrequency = 1.0;

	    /// Default lacunarity for the noise::module::Perlin noise module.
	    public const double DefaultPerlinLacunarity = 2.0;

	    /// Default number of octaves for the noise::module::Perlin noise module.
	    public const int DefaultPerlinOctaveCount = 6;

	    /// Default persistence value for the noise::module::Perlin noise module.
	    public const double DefaultPerlinPersistence = 0.5;

	    /// Default noise quality for the noise::module::Perlin noise module.
	    public const NoiseQuality DefaultPerlinQuality = NoiseQuality.Standard;

	    /// Default noise seed for the noise::module::Perlin noise module.
	    public const int DefaultPerlinSeed = 0;

	    /// Maximum number of octaves for the noise::module::Perlin noise module.
        public const int PerlinMaxOctave = 30;        	   
	   
	    /// Total number of octaves that generate the Perlin noise.
	    int octaveCount = DefaultPerlinOctaveCount;
	 

        public double Frequency
        {
            get;
            set;
        }
        public int OctaveCount 
        {
            get { return octaveCount; }
            set { octaveCount = Math.Max(0, Math.Min(value, PerlinMaxOctave));  }
        }
        public double Persistence
        {
            get;
            set;
        }
        public double Lacunarity
        {
            get;
            set;
        }
        public NoiseQuality Quality
        {
            get;
            set;
        }

        public Perlin(int seed)
        {
            Frequency = DefaultPerlinFrequency;
            OctaveCount = DefaultPerlinOctaveCount;
            Persistence = DefaultPerlinPersistence;
            Lacunarity = DefaultPerlinLacunarity;
            Quality = DefaultPerlinQuality;
            Seed = seed;
        }

        public Perlin()
        {
            Frequency = DefaultPerlinFrequency;
            OctaveCount = DefaultPerlinOctaveCount;
            Persistence = DefaultPerlinPersistence;
            Lacunarity = DefaultPerlinLacunarity;
            Quality = DefaultPerlinQuality;
            Seed = DefaultPerlinSeed;
        }

        public override double GetValue(double x, double y, double z)
        {
            double value = 0.0;
            double curPersistence = 1.0;
            //double nx, ny, nz;

            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (int currentOctave = 0; currentOctave < OctaveCount; currentOctave++)
            {
                long seed = (Seed + currentOctave) & 0xffffffff;
                /*nx = Math.MakeInt32Range(x);
                ny = Math.MakeInt32Range(y);
                nz = Math.MakeInt32Range(z);*/
                double signal = GeneratorBase.GradientCoherentNoise(x, y, z, (int) seed, Quality);
                //signal = cachedNoise3(x, y, z);

                value += signal * curPersistence;

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                curPersistence *= Persistence;
            }
            return value;
        }
    }
}