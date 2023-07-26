using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Vectors
{
    public struct Vector3<T> : IVector3<T>
    {
        public Vector3(T x, T y, T z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public T X { get; set; }

        public T Y { get; set; }

        public T Z { get; set; }

        public override string ToString()
        {
            return $"[{X},{Y},{Z}]";
        }
    }
}
