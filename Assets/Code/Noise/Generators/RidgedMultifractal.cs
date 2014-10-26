using System;
using Voxel.Noise.Util;
using UnityEngine;

namespace Voxel.Noise.Generators
{
    public class RidgedMultifractal : NoiseModule
    {
        public float Frequency = 1.0f;
        public NoiseQuality NoiseQuality = NoiseQuality.Standard;
        private int octaveCount;
        private float lacunarity;

        private const int MaxOctaves = 30;

        private readonly float[] SpectralWeights = new float[MaxOctaves];

        public RidgedMultifractal()
        {
            Frequency = 1.0f;
            Lacunarity = 2.0f;
            OctaveCount = 6;
            NoiseQuality = NoiseQuality.Standard;
            Seed = 0;
        }

        public override double GetValue(double x, double y, double z)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            double value = 0.0;
            double weight = 1.0;

            const double offset = 1.0;
            const double gain = 2.0;

            for (int currentOctave = 0; currentOctave < OctaveCount; currentOctave++)
            {
                //double nx, ny, nz;

                /* nx = Math.MakeInt32Range(x);
                 ny = Math.MakeInt32Range(y);
                 nz = Math.MakeInt32Range(z);*/

                long seed = (Seed + currentOctave) & 0x7fffffff;
                double signal = GeneratorBase.GradientCoherentNoise(x, y, z,
                    (int)seed, NoiseQuality);

                // Make the ridges.
                signal = Math.Abs(signal);
                signal = offset - signal;

                // Square the signal to increase the sharpness of the ridges.
                signal *= signal;

                // The weighting from the previous octave is applied to the signal.
                // Larger values have higher weights, producing sharp points along the
                // ridges.
                signal *= weight;

                // Weight successive contributions by the previous signal.
                weight = signal * gain;
                if (weight > 1.0)
                {
                    weight = 1.0;
                }
                if (weight < 0.0)
                {
                    weight = 0.0;
                }

                // Add the signal to the output value.
                value += (signal * SpectralWeights[currentOctave]);

                // Go to the next octave.
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            return (value * 1.25) - 1.0;
        }

        public float Lacunarity
        {
            get { return lacunarity; }
            set
            {
                lacunarity = value;
                CalculateSpectralWeights();
            }
        }

        public int OctaveCount
        {
            get { return octaveCount; }
            set
            {
                if (value < 1 || value > MaxOctaves)
                    throw new ArgumentException("Octave count must be greater than zero and less than " + MaxOctaves);

                octaveCount = value;
            }
        }

        private void CalculateSpectralWeights()
        {
            const float h = 1.0f;

            float frequency = 1.0f;
            for (int i = 0; i < MaxOctaves; i++)
            {
                // Compute weight for each frequency.

                SpectralWeights[i] = Mathf.Pow(frequency, -h);
                frequency *= lacunarity;
            }
        }
    }
}