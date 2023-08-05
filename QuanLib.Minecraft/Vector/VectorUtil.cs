using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Vector
{
    public static class VectorUtil
    {
        public static double ManhattanDistance(IVector3<double> value1, IVector3<double> value2)
        {
            return Math.Abs(value1.X - value2.X) + Math.Abs(value1.Y - value2.Y) + Math.Abs(value1.Z - value2.Z);
        }
    }
}
