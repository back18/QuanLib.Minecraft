using QuanLib.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Selectors
{
    public class Vector3TargetArgument<T> : TargetArgument<Vector3<T>> where T : INumber<T>, IConvertible
    {
        public Vector3TargetArgument(Vector3<T> position) : base(position)
        {
            Position = position;
        }

        public Vector3<T> Position { get; }

        public static implicit operator Vector3TargetArgument<T>(Vector3<T> value)
        {
            return new(value);
        }

        public string ToString(string x, string y, string z)
        {
            return $"{new TargetArgument<T>(Position.X).ToString(x)},{new TargetArgument<T>(Position.Y).ToString(y)},{new TargetArgument<T>(Position.Z).ToString(z)}";
        }
    }
}
