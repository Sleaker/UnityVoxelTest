using UnityEngine;
using Random = System.Random;

namespace Voxel.Noise.Generators
{
    public class SysRandom : NoiseModule
    {
        public override double GetValue(double x, double y, double z)
        {
            Random rng = new Random(Seed ^ Mathf.RoundToInt((float)x) * Mathf.RoundToInt((float)y) ^ Mathf.RoundToInt((float)z));
            return rng.NextDouble();
        }
    }
}
