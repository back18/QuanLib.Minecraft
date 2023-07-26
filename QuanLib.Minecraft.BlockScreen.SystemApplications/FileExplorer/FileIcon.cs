using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.FileExplorer
{
    public class FileIcon : PathIcon
    {
        public FileIcon(FileInfo fileInfo) : base(IO.PathType.File)
        {
            FileInfo = fileInfo;

            Text = fileInfo.Name;
            Skin.BackgroundBlockID = ConcretePixel.ToBlockID(MinecraftColor.White);
            Skin.BackgroundBlockID_Hover = ConcretePixel.ToBlockID(MinecraftColor.White);
            Skin.BackgroundBlockID_Selected = ConcretePixel.ToBlockID(MinecraftColor.LightGray);
            Skin.BackgroundBlockID_Hover_Selected = ConcretePixel.ToBlockID(MinecraftColor.LightGray);
        }

        public override string Path => FileInfo.FullName;

        public FileInfo FileInfo { get; }
    }
}
