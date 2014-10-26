namespace Voxel.Noise.Modifiers
{
    public class Bias : NoiseModule
    {
        public NoiseModule SourceModule;
        public double bias = 0;

        public override double GetValue(double x, double y, double z)
        {
            if (SourceModule == null)
                throw new System.NullReferenceException("A source module must be provided.");

            return SourceModule.GetValue(x, y, z) + bias;
        }
    }
}
