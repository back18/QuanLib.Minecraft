using QuanLib.Minecraft.BlockScreen.UI;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public abstract class AbstractFrame
    {
        public abstract int Width { get; }

        public abstract int Height { get; }

        public abstract ArrayFrame ToArrayFrame();

        public abstract LinkedFrame ToLinkedFrame();

        public abstract void CorrectSize(Size size, ContentAnchor anchor, string background);
    }
}
