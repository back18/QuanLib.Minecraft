using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class ChatMessage
    {
        public ChatMessage(string sender, string message)
        {
            Sender = sender ?? throw new ArgumentNullException(nameof(sender));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Sender { get; }

        public string Message { get; }

        public override string ToString()
        {
            return $"<{Sender}> {Message}";
        }
    }
}
