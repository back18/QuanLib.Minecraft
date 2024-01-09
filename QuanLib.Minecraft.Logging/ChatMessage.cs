using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Logging
{
    public class ChatMessage
    {
        public ChatMessage(string sender, string message)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));
            ArgumentNullException.ThrowIfNull(message, nameof(message));

            Sender = sender;
            Message = message;
        }

        public string Sender { get; }

        public string Message { get; }

        public override string ToString()
        {
            return $"<{Sender}> {Message}";
        }
    }
}
