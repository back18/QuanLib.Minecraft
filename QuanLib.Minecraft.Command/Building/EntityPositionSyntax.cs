using QuanLib.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class EntityPositionSyntax<TNext>(ICommandSyntax? previous) : CommandSyntax(previous) where TNext : ICreatible<TNext>
    {
        public TNext Zero()
        {
            SetSyntax("0 0 0");
            return TNext.Create(this);
        }

        public TNext SetPosition<T>(T position) where T : IVector3<double>
        {
            return SetPosition(position.X, position.Y, position.Z);
        }

        public TNext SetPosition(double x, double y, double z)
        {
            SetSyntax($"{x} {y} {z}");
            return TNext.Create(this);
        }
    }
}
