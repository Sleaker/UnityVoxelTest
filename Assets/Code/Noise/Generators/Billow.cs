using Voxel.Noise.Util;

namespace Voxel.Noise.Generators
{
    public class Billow : NoiseModule
    {

        public double Frequency = 1.0;
        public double Persistence = 0.5;
        public NoiseQuality NoiseQuality = NoiseQuality.Standard;
        public int OctaveCount = 8;
        public double Lacunarity = 2.0;

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
                double signal = GeneratorBase.GradientCoherentNoise(x, y, z, (int)seed, NoiseQuality);
                signal = 2.0 * System.Math.Abs(signal) - 1.0;
                value += signal * curPersistence;

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                curPersistence *= Persistence;
            }

            value += 0.5;

            return value;
        }
    }
}
