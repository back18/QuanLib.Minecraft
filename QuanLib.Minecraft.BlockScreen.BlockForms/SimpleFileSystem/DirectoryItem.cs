using QuanLib.Minecraft.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms.SimpleFileSystem
{
    public class DirectoryItem : FileSystemItem
    {
        public DirectoryItem(DirectoryInfo directoryInfo) : base(Core.IO.PathType.Directory)
        {
            DirectoryInfo = directoryInfo ?? throw new ArgumentNullException(nameof(directoryInfo));

            Text = directoryInfo.Name;
            Skin.BackgroundBlockID = BlockManager.Concrete.Yellow;
            Skin.BackgroundBlockID_Hover = BlockManager.Concrete.Yellow;
            Skin.BackgroundBlockID_Selected = BlockManager.Concrete.Orange;
            Skin.BackgroundBlockID_Hover_Selected = BlockManager.Concrete.Orange;
        }

        public override FileSystemInfo FileSystemInfo => DirectoryInfo;

        public DirectoryInfo DirectoryInfo { get; }
    }
}
