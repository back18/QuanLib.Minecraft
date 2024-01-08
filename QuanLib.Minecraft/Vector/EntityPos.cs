using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Vector
{
    public struct EntityPos : IVector3<double>
    {
        public EntityPos(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public EntityPos(IVector3<double> vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public readonly BlockPos ToBlockPos()
        {
            int x = (int)Math.Round(X, MidpointRounding.ToNegativeInfinity);
            int y = (int)Math.Round(Y, MidpointRounding.ToNegativeInfinity);
            int z = (int)Math.Round(Z, MidpointRounding.ToNegativeInfinity);
            return new(x, y, z);
        }

        public static bool CheckPlaneReachability(EntityPos position, Rotation rotation, Facing normalFacing, int target)
        {
            if (!rotation.Contains(normalFacing.Reverse()))
                return false;

            position.Y += 1.625;
            switch (normalFacing)
            {
                case Facing.Xp:
                    return position.X > target;
                case Facing.Xm:
                    return position.X < target;
                case Facing.Yp:
                    return position.Y > target;
                case Facing.Ym:
                    return position.Y < target;
                case Facing.Zp:
                    return position.Z > target;
                case Facing.Zm:
                    return position.Z < target;
                default:
                    throw new InvalidOperationException();
            }
        }

        public static EntityPos GetToPlaneIntersection(EntityPos position, Vector3<double> direction, Facing planeFacing, int target)
        {
            int targetOffset = target;
            if (planeFacing > 0)
                targetOffset += 1;

            // 计算t值，t值代表射线源点到交点的距离与射线方向向量的比值
            double t, x, y, z;
            switch (planeFacing)
            {
                case Facing.Yp:
                case Facing.Ym:
                    t = (targetOffset - position.Y) / direction.Y;
                    x = position.X + direction.X * t;
                    y = target;
                    z = position.Z + direction.Z * t;
                    break;
                case Facing.Zp:
                case Facing.Zm:
                    t = (targetOffset - position.Z) / direction.Z;
                    x = position.X + direction.X * t;
                    y = position.Y + direction.Y * t;
                    z = target;
                    break;
                case Facing.Xp:
                case Facing.Xm:
                    t = (targetOffset - position.X) / direction.X;
                    x = target;
                    y = position.Y + direction.Y * t; ;
                    z = position.Z + direction.Z * t;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return new(x, y, z);
        }

        public static EntityPos operator -(EntityPos a)
        {
            return new(0 - a.X, 0 - a.Y, 0 - a.Z);
        }

        public static EntityPos operator +(EntityPos a, EntityPos b)
        {
            return new EntityPos(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static EntityPos operator -(EntityPos a, EntityPos b)
        {
            return new EntityPos(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static EntityPos operator *(EntityPos a, EntityPos b)
        {
            return new EntityPos(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static EntityPos operator /(EntityPos a, EntityPos b)
        {
            return new EntityPos(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static EntityPos operator +(EntityPos a, double b)
        {
            return new(a.X + b, a.Y + b, a.Z + b);
        }

        public static EntityPos operator -(EntityPos a, double b)
        {
            return new(a.X - b, a.Y - b, a.Z - b);
        }

        public static EntityPos operator *(EntityPos a, double b)
        {
            return new(a.X * b, a.Y * b, a.Z * b);
        }

        public static EntityPos operator /(EntityPos a, double b)
        {
            return new(a.X / b, a.Y / b, a.Z / b);
        }

        public static bool operator ==(EntityPos v1, EntityPos v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
        }

        public static bool operator !=(EntityPos v1, EntityPos v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z;
        }

        public static implicit operator EntityPos(Vector3<double> v)
        {
            return new(v.X, v.Y, v.Z);
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj is EntityPos other)
            {
                return Equals(other);
            }

            return false;
        }

        public readonly bool Equals(EntityPos other)
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
