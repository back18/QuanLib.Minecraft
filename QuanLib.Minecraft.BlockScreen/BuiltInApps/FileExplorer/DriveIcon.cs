using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.FileExplorer
{
    public class DriveIcon : PathIcon
    {
        public DriveIcon(DriveInfo driveInfo) : base(IO.PathType.Drive)
        {
            DriveInfo = driveInfo ?? throw new ArgumentNullException(nameof(driveInfo));

            Text = driveInfo.Name;
            Skin.BackgroundBlockID = ConcretePixel.ToBlockID(MinecraftColor.LightBlue);
            Skin.BackgroundBlockID_Hover = ConcretePixel.ToBlockID(MinecraftColor.LightBlue);
            Skin.BackgroundBlockID_Selected = ConcretePixel.ToBlockID(MinecraftColor.Blue);
            Skin.BackgroundBlockID_Hover_Selected = ConcretePixel.ToBlockID(MinecraftColor.Blue);
        }

        public override string Path => DriveInfo.RootDirectory.FullName;

        public DriveInfo DriveInfo { get; }
    }
}
