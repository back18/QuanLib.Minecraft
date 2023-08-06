using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Album
{
    public class AlbumApp : Application
    {
        public const string ID = "Album";

        public const string Name = "相册";

        public override object? Main(string[] args)
        {
            RunForm(new AlbumForm());
            return null;
        }
    }
}
