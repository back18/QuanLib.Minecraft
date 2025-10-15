using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class PositionedOverSyntax(ICommandSyntax? previous) : CommandSyntax(previous)
    {
        public ExecuteCommandSyntax MotionBlocking()
        {
            SetSyntax("motion_blocking");
            return new ExecuteCommandSyntax(this);
        }

        public ExecuteCommandSyntax MotionBlockingNoLeaves()
        {
            SetSyntax("motion_blocking_no_leaves");
            return new ExecuteCommandSyntax(this);
        }

        public ExecuteCommandSyntax OceanFloor()
        {
            SetSyntax("ocean_floor");
            return new ExecuteCommandSyntax(this);
        }

        public ExecuteCommandSyntax WorldSurface()
        {
            SetSyntax("world_surface");
            return new ExecuteCommandSyntax(this);
        }
    }
}
