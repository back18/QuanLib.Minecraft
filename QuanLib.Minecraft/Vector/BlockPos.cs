using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Vector
{
    public struct BlockPos : IVector3<int>
    {
        public BlockPos(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public BlockPos(IVector3<int> vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        public static BlockPos operator -(BlockPos a)
        {
            return new(0 - a.X, 0 - a.Y, 0 - a.Z);
        }

        public static BlockPos operator +(BlockPos a, BlockPos b)
        {
            return new BlockPos(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static BlockPos operator -(BlockPos a, BlockPos b)
        {
            return new BlockPos(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static BlockPos operator *(BlockPos a, BlockPos b)
        {
            return new BlockPos(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static BlockPos operator /(BlockPos a, BlockPos b)
        {
            return new BlockPos(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static BlockPos operator +(BlockPos a, int b)
        {
            return new(a.X + b, a.Y + b, a.Z + b);
        }

        public static BlockPos operator -(BlockPos a, int b)
        {
            return new(a.X - b, a.Y - b, a.Z - b);
        }

        public static BlockPos operator *(BlockPos a, int b)
        {
            return new(a.X * b, a.Y * b, a.Z * b);
        }

        public static BlockPos operator /(BlockPos a, int b)
        {
            return new(a.X / b, a.Y / b, a.Z / b);
        }

        public static bool operator ==(BlockPos v1, BlockPos v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
        }

        public static bool operator !=(BlockPos v1, BlockPos v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z;
        }

        public static implicit operator BlockPos(Vector3<int> v)
        {
            return new(v.X, v.Y, v.Z);
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj is BlockPos other)
            {
                return Equals(other);
            }

            return false;
        }

        public readonly bool Equals(BlockPos other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
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
