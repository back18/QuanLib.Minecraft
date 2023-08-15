using FFMediaToolkit.Decoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Event
{
    public class MediaFilePlayerChangedEventArge : EventArgs
    {
        public MediaFilePlayerChangedEventArge(MediaFilePlayer? oldMediaFilePlayer, MediaFilePlayer? newMediaFilePlayer)
        {
            OldMediaFilePlayer = oldMediaFilePlayer;
            NewMediaFilePlayer = newMediaFilePlayer;
        }

        public MediaFilePlayer? OldMediaFilePlayer { get; }

        public MediaFilePlayer? NewMediaFilePlayer { get; }
    }
}
