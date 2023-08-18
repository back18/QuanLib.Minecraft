using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Screens
{
    public interface IScreenOptions
    {
        public Vector3<int> StartPosition { get; }

        public int Width { get; }

        public int Height { get; }

        public Facing XFacing { get; }

        public Facing YFacing { get; }
    }
}
