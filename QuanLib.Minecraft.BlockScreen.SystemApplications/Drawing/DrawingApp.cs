using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Drawing
{
    public class DrawingApp : Application
    {
        public const string ID = "Drawing";

        public const string Name = "绘画";

        public override object? Main(string[] args)
        {
            RunForm(new DrawingForm());
            return null;
        }
    }
}
