using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Event
{
    public class ChatMessageEventArgs : EventArgs
    {
        public ChatMessageEventArgs(ChatMessage chatMessage)
        {
            ChatMessage = chatMessage;
        }

        public ChatMessage ChatMessage { get; }
    }
}
