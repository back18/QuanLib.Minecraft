using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Event
{
    public class VideoFrameChangedEventArgs : EventArgs
    {
        public VideoFrameChangedEventArgs(VideoFrame videoFrame)
        {
            VideoFrame = videoFrame;
        }

        public VideoFrame VideoFrame { get; }
    }
}
