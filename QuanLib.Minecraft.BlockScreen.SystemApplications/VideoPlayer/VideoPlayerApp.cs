using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.VideoPlayer
{
    public class VideoPlayerApp : Application
    {
        public const string ID = "VideoPlayer";

        public const string Name = "视频播放器";

        public override object? Main(string[] args)
        {
            string? path = null;
            if (args.Length > 0)
                path = args[0];

            RunForm(new VideoPlayerForm(path));
            return null;
        }
    }
}
