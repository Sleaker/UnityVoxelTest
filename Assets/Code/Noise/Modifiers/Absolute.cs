namespace Voxel.Noise.Modifiers
{
    public class Absolute : NoiseModule
    {
        public NoiseModule SourceModule = null;

        public override double GetValue(double x, double y, double z)
        {
            if (SourceModule == null)
                throw new System.NullReferenceException("A source module must be provided.");

            return System.Math.Abs(SourceModule.GetValue(x, y, z));
        }
    }
}
