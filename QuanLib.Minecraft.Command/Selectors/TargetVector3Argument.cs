using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Selectors
{
    public class TargetVector3Argument<T> : TargetArgument<Vector3<T>> where T : notnull
    {
        public TargetVector3Argument(Vector3<T> position) : base(position)
        {
            Position = position;
        }

        public Vector3<string> Name { get; }

        public Vector3<T> Position { get; }

        public static implicit operator TargetVector3Argument<T>(Vector3<T> value)
        {
            return new(value);
        }

        public string ToString(Vector3<string> name)
        {
            return $"{new TargetArgument<T>(Position.X).ToString(name.X)},{new TargetArgument<T>(Position.Y).ToString(name.Y)},{new TargetArgument<T>(Position.Z).ToString(name.Z)}";
        }
    }
}
