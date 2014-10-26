using System;

namespace Voxel.Noise.Modifiers
{
    public class Displace : NoiseModule
    {
        public NoiseModule SourceModule
        {
            get;
            set;
        }

        public NoiseModule XDisplaceModule
        {
            get;
            set;
        }

        public NoiseModule YDisplaceModule
        {
            get;
            set;
        }

        public NoiseModule ZDisplaceModule
        {
            get;
            set;
        }

        public void SetDisplaceModules(NoiseModule x, NoiseModule y, NoiseModule z)
        {
            XDisplaceModule = x;
            YDisplaceModule = y;
            ZDisplaceModule = z;
        }

        public override double GetValue(double x, double y, double z)
        {
            if (SourceModule == null)
                throw new InvalidOperationException("Source Module cannot be null");
            if (XDisplaceModule == null)
                throw new InvalidOperationException("XDisplace Module cannot be null");
            if (YDisplaceModule == null)
                throw new InvalidOperationException("YDisplace Module cannot be null");
            if (ZDisplaceModule == null)
                throw new InvalidOperationException("ZDisplace Module cannot be null");

            // Get the output values from the three displacement modules.  Add each
            // value to the corresponding coordinate in the input value.
            double xDisplace = x + (XDisplaceModule.GetValue(x, y, z));
            double yDisplace = y + (YDisplaceModule.GetValue(x, y, z));
            double zDisplace = z + (ZDisplaceModule.GetValue(x, y, z));

            // Retrieve the output value using the offsetted input value instead of
            // the original input value.
            return SourceModule.GetValue(xDisplace, yDisplace, zDisplace);
        }
    }
}
