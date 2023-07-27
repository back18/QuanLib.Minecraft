using QuanLib.IO;
using QuanLib.Minecraft.BlockScreen.BlockForms;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.FileExplorer
{
    public abstract class PathIcon : Switch
    {
        protected PathIcon(PathType pathType)
        {
            PathType = pathType;

            AutoSize = true;
            Skin.BorderBlockID__Hover = ConcretePixel.ToBlockID(MinecraftColor.Pink);
            Skin.BorderBlockID_Hover_Selected = ConcretePixel.ToBlockID(MinecraftColor.Pink);
            CursorEnter += PathIcon_CursorEnter;
            CursorLeave += PathIcon_CursorLeave;
        }

        public PathType PathType { get; }

        public abstract string Path { get; }

        private void PathIcon_CursorEnter(Point position)
        {
            BorderWidth = 2;
            Location = ClientLocation;
        }

        private void PathIcon_CursorLeave(Point position)
        {
            BorderWidth = 1;
            ClientLocation = Location;
        }
    }
}
