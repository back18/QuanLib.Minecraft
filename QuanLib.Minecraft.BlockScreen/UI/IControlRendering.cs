using QuanLib.Minecraft.BlockScreen.Frame;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI
{
    public interface IControlRendering
    {
        public bool Visible { get; set; }

        public Point RenderingLocation { get; set; }

        public Size RenderingSize { get; set; }

        public int BorderWidth { get; set; }

        public AnchorPosition ContentAnchor { get; set; }

        public ISkin Skin { get; }

        public IFrame RenderingFrame();

        public bool NeedRendering();

        public ArrayFrame? GetFrameCache();

        public void OnRenderingCompleted(ArrayFrame frame);
    }
}
