using System;
using Voxel.Noise.Util;

namespace Voxel.Noise.Modifiers
{
    public class Blend : NoiseModule
    {
        public NoiseModule SourceModule1;
        public NoiseModule SourceModule2;
        public NoiseModule WeightModule;

        public Blend(NoiseModule sourceModule1, NoiseModule sourceModule2, NoiseModule weightModule)
        {
            if (sourceModule1 == null || sourceModule2 == null || weightModule == null)
                throw new ArgumentNullException("No source Modules may be null");

            SourceModule1 = sourceModule1;
            SourceModule2 = sourceModule2;
            WeightModule = weightModule;
        }


        public override double GetValue(double x, double y, double z)
        {
            if (SourceModule1 == null || SourceModule2 == null || WeightModule == null)
                throw new NullReferenceException("No source module can be null");

            return NoiseMath.LinearInterpolate(SourceModule1.GetValue(x, y, z), SourceModule2.GetValue(x, y, z),
                (WeightModule.GetValue(x, y, z) + 1.0) / 2.0);
        }
    }
}

