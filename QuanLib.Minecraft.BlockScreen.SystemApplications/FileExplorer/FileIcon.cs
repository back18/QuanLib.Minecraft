using QuanLib.Minecraft.Block;
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
            Skin.BackgroundBlockID = BlockManager.Concrete.White;
            Skin.BackgroundBlockID_Hover = BlockManager.Concrete.White;
            Skin.BackgroundBlockID_Selected = BlockManager.Concrete.LightGray;
            Skin.BackgroundBlockID_Hover_Selected = BlockManager.Concrete.LightGray;
        }

        public override string Path => FileInfo.FullName;

        public FileInfo FileInfo { get; }
    }
}
