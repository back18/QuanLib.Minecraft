using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.ImageViewer
{
    public class ImageViewerApp : Application
    {
        public const string ID = "ImageViewer";

        public const string Name = "图片浏览器";

        public override object? Main(string[] args)
        {
            RunForm(new ImageViewerForm());
            return null;
        }
    }
}
