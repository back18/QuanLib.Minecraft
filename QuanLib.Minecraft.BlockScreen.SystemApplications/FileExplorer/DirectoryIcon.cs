using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.FileExplorer
{
    public class DirectoryIcon : PathIcon
    {
        public DirectoryIcon(DirectoryInfo directoryInfo) : base(IO.PathType.Directory)
        {
            DirectoryInfo = directoryInfo ?? throw new ArgumentNullException(nameof(directoryInfo));

            Text = directoryInfo.Name;
            Skin.BackgroundBlockID = ConcretePixel.ToBlockID(MinecraftColor.Yellow);
            Skin.BackgroundBlockID_Hover = ConcretePixel.ToBlockID(MinecraftColor.Yellow);
            Skin.BackgroundBlockID_Selected = ConcretePixel.ToBlockID(MinecraftColor.Orange);
            Skin.BackgroundBlockID_Hover_Selected = ConcretePixel.ToBlockID(MinecraftColor.Orange);
        }

        public override string Path => DirectoryInfo.FullName;

        public DirectoryInfo DirectoryInfo { get; }
    }
}
