namespace Voxel.Noise
{
    public enum NoiseQuality
    {
        Low,
        Standard,
        High
    }

    public abstract class NoiseModule
    {
        protected int seed = 0;
        public virtual int Seed { get { return seed; } set { seed = value; } }

        public float GetValue(float x, float y, float z)
        {
            return (float)this.GetValue((double)x, (double)y, (double)z);
        }


        public abstract double GetValue(double x, double y, double z);

    }
}