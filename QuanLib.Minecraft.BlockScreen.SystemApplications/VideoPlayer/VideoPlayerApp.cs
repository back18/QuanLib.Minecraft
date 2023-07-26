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
        public VideoPlayerApp()
        {
            VideoPlayerForm = new VideoPlayerForm();
            MainForm = VideoPlayerForm;
            _exit = new(false);
        }

        private readonly AutoResetEvent _exit;

        public const string ID = "VideoPlayer";

        public const string Name = "视频播放器";

        private readonly VideoPlayerForm VideoPlayerForm;

        public override IForm MainForm { get; }

        public override object? Main(string[] args)
        {
            if (args.Length > 0)
            {
                VideoPlayerForm.Open(args[0]);
            }
            _exit.WaitOne();
            return null;
        }

        public override void Exit()
        {
            _exit.Set();
            base.Exit();
        }
    }
}
