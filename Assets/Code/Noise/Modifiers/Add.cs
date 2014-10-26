namespace Voxel.Noise.Modifiers
{
    public class Add : NoiseModule
    {
        public NoiseModule SourceModule1;
        public NoiseModule SourceModule2;


        public override double GetValue(double x, double y, double z)
        {
            if (SourceModule1 == null || SourceModule2 == null)
                throw new System.NullReferenceException("Source modules must be provided.");

            return SourceModule1.GetValue(x, y, z) + SourceModule2.GetValue(x, y, z);
        }
    }
}
