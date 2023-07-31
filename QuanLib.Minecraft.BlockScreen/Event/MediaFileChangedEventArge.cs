using FFMediaToolkit.Decoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Event
{
    public class MediaFileChangedEventArge : EventArgs
    {
        public MediaFileChangedEventArge(MediaFile? mediaFile)
        {
            MediaFile = mediaFile;
        }

        public MediaFile? MediaFile { get; }
    }
}
