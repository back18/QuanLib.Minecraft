using QuanLib.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public struct Bounds
    {
        public Bounds(Vector3<double> startPosition, Vector3<double> range)
        {
            StartPosition = startPosition;
            Range = range;
        }

        public Vector3<double> StartPosition { get; set; }

        public Vector3<double> Range { get; set; }

        public readonly Vector3<double> EndPosition => new(StartPosition.X + Range.X, StartPosition.Y + Range.Y, StartPosition.Z + Range.Z);

        public readonly bool Contains<T>(T position) where T : IVector3<double>
        {
            ArgumentNullException.ThrowIfNull(position, nameof(position));

            Vector3<double> start = StartPosition;
            Vector3<double> end = EndPosition;
            return
                position.X >= start.X &&
                position.Y >= start.Y &&
                position.Z >= start.Z &&
                position.X <= end.X &&
                position.Y <= end.Y &&
                position.Z <= end.Z;
        }
    }
}
