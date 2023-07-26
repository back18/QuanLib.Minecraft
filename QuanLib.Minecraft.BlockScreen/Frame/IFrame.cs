using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Frame
{
    public interface IFrame
    {
        public int Width { get; }

        public int Height { get; }

        public ArrayFrame ToArrayFrame();

        public LinkedFrame ToLinkedFrame();

        public void CorrectSize(Size size, AnchorPosition anchor, string background);
    }
}
