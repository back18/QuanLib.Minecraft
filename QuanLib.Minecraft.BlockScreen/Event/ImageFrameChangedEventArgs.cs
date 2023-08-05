using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Event
{
    public class ImageFrameChangedEventArgs : EventArgs
    {
        public ImageFrameChangedEventArgs(ImageFrame oldImageFrame, ImageFrame newImageFrame)
        {
            OldImageFrame = oldImageFrame;
            NewImageFrame = newImageFrame;
        }

        public ImageFrame OldImageFrame { get; }

        public ImageFrame NewImageFrame { get; }
    }
}
