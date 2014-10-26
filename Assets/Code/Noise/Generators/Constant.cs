namespace Voxel.Noise.Generators
{
    public class Constant : NoiseModule
    {
        public double Value = 0;
        public override double GetValue(double x, double y, double z)
        {
            return Value;
        }
    }
}
