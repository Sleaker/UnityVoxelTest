using System;

namespace Voxel.Exceptions
{
    public class BlockNotFoundException : Exception
    {
        public BlockNotFoundException(string invalid) : base("Block named: " + invalid + " was not found in the BlockRegistry!")
        {            
        }
    }
}
