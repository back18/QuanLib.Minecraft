using QuanLib.Minecraft.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.FileExplorer
{
    public class DriveIcon : PathIcon
    {
        public DriveIcon(DriveInfo driveInfo) : base(IO.PathType.Drive)
        {
            DriveInfo = driveInfo ?? throw new ArgumentNullException(nameof(driveInfo));

            Text = driveInfo.Name;
            Skin.BackgroundBlockID = BlockManager.Concrete.LightBlue;
            Skin.BackgroundBlockID_Hover = BlockManager.Concrete.LightBlue;
            Skin.BackgroundBlockID_Selected = BlockManager.Concrete.Blue;
            Skin.BackgroundBlockID_Hover_Selected = BlockManager.Concrete.Blue;
        }

        public override string Path => DriveInfo.RootDirectory.FullName;

        public DriveInfo DriveInfo { get; }
    }
}
