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
        public bool Visible { get; }

        public ISkin Skin { get; }

        public Point RenderingLocation { get; }

        public Size RenderingSize { get; }

        public int BorderWidth { get; }

        public ContentAnchor ContentAnchor { get; }

        public AbstractFrame RenderingFrame();

        public bool NeedRendering();

        public ArrayFrame? GetFrameCache();

        public void OnRenderingCompleted(ArrayFrame frame);

        public IEnumerable<IControlRendering> GetSubRenderings();
    }
}
