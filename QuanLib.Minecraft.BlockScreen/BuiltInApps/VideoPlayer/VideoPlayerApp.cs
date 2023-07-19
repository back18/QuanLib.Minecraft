using QuanLib.Minecraft.BlockScreen.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.VideoPlayer
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

        public override string AppID => ID;

        public override string AppName => Name;

        private readonly VideoPlayerForm VideoPlayerForm;

        public override Form MainForm { get; }

        public override object? Main(string[] args)
        {
            if (args.Length >0)
            {
                VideoPlayerForm.Open(args[0]);
            }
            _exit.WaitOne();
            return null;
        }

        public override void Exit()
        {
            _exit.Set();
        }
    }
}
