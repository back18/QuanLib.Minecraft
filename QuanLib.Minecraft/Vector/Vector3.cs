using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Vector
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

        public static bool operator ==(Vector3<T> v1, Vector3<T> v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Vector3<T> v1, Vector3<T> v2)
        {
            return !v1.Equals(v2);
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj is Vector3<T> other)
            {
                return Equals(other);
            }

            return false;
        }

        public readonly bool Equals(Vector3<T> other)
        {
            return EqualityComparer<T>.Default.Equals(X, other.X)
                   && EqualityComparer<T>.Default.Equals(Y, other.Y)
                   && EqualityComparer<T>.Default.Equals(Z, other.Z);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public override readonly string ToString()
        {
            return $"[{X},{Y},{Z}]";
        }
    }
}
