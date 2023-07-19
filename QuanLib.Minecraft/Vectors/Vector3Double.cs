using QuanLib.Minecraft.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Vectors
{
    public struct Vector3Double : IVector3<double>
    {
        public Vector3Double(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public static Vector3Double GetVector3Double(Vector3<double> vector)
        {
            return new(vector.X, vector.Y, vector.Z);
        }

        public Vector3<int> ToVector3Int()
        {
            int x = (int)Math.Round(X, MidpointRounding.ToNegativeInfinity);
            int y = (int)Math.Round(Y, MidpointRounding.ToNegativeInfinity);
            int z = (int)Math.Round(Z, MidpointRounding.ToNegativeInfinity);
            return new(x, y, z);
        }

        public static bool CheckPlaneReachability(Vector3Double position, Rotation rotation, Facing normalFacing, int target)
        {
            if (!rotation.Contain((Facing)(-(int)normalFacing)))
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

        public static Vector3Double GetToPlaneIntersection(Vector3Double position, Rotation rotation, Facing planeFacing, int target)
        {
            position.Y += 1.625;
            Vector3Double direction = rotation.ToDirection();

            return GetToPlaneIntersection(position, direction, planeFacing, target);
        }

        public static Vector3Double GetToPlaneIntersection(Vector3Double position, Vector3Double direction, Facing planeFacing, int target)
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

        public static Vector3Double operator -(Vector3Double a)
        {
            return new(0 - a.X, 0 - a.Y, 0 - a.Z);
        }

        public static Vector3Double operator +(Vector3Double a, double b)
        {
            return new(a.X + b, a.Y + b, a.Z + b);
        }

        public static Vector3Double operator -(Vector3Double a, double b)
        {
            return new(a.X - b, a.Y - b, a.Z - b);
        }

        public static Vector3Double operator *(Vector3Double a, double b)
        {
            return new(a.X * b, a.Y * b, a.Z * b);
        }

        public static Vector3Double operator /(Vector3Double a, double b)
        {
            return new(a.X / b, a.Y / b, a.Z / b);
        }

        public static Vector3Double operator +(Vector3Double a, Vector3Double b)
        {
            return new Vector3Double(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3Double operator -(Vector3Double a, Vector3Double b)
        {
            return new Vector3Double(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3Double operator *(Vector3Double a, Vector3Double b)
        {
            return new Vector3Double(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Vector3Double operator /(Vector3Double a, Vector3Double b)
        {
            return new Vector3Double(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }
    }
}
