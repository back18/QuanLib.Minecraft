using QuanLib.Minecraft.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms.SimpleFileSystem
{
    public class FileItem : FileSystemItem
    {
        public FileItem(FileInfo fileInfo) : base(Core.IO.PathType.File)
        {
            FileInfo = fileInfo;

            Text = fileInfo.Name;
            Skin.BackgroundBlockID = BlockManager.Concrete.White;
            Skin.BackgroundBlockID_Hover = BlockManager.Concrete.White;
            Skin.BackgroundBlockID_Selected = BlockManager.Concrete.LightGray;
            Skin.BackgroundBlockID_Hover_Selected = BlockManager.Concrete.LightGray;
        }

        public override FileSystemInfo FileSystemInfo => FileInfo;

        public FileInfo FileInfo { get; }
    }
}
