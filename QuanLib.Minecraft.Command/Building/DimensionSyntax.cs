using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class DimensionSyntax(ICommandSyntax? previous) : CommandSyntax(previous)
    {
        public ExecuteCommandSyntax Overworld()
        {
            SetSyntax("minecraft:overworld");
            return new ExecuteCommandSyntax(this);
        }

        public ExecuteCommandSyntax Nether()
        {
            SetSyntax("minecraft:the_nether");
            return new ExecuteCommandSyntax(this);
        }

        public ExecuteCommandSyntax TheEnd()
        {
            SetSyntax("minecraft:the_end");
            return new ExecuteCommandSyntax(this);
        }

        public ExecuteCommandSyntax SetDimension(string dimension)
        {
            ArgumentException.ThrowIfNullOrEmpty(dimension, nameof(dimension));

            SetSyntax(dimension);
            return new ExecuteCommandSyntax(this);
        }
    }
}
