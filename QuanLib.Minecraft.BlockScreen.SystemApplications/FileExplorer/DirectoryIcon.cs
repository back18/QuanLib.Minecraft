using QuanLib.Minecraft.Block;
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
            Skin.BackgroundBlockID = BlockManager.Concrete.Yellow;
            Skin.BackgroundBlockID_Hover = BlockManager.Concrete.Yellow;
            Skin.BackgroundBlockID_Selected = BlockManager.Concrete.Orange;
            Skin.BackgroundBlockID_Hover_Selected = BlockManager.Concrete.Orange;
        }

        public override string Path => DirectoryInfo.FullName;

        public DirectoryInfo DirectoryInfo { get; }
    }
}
