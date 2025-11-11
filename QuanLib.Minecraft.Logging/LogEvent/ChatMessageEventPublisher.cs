using QuanLib.Core.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Logging.LogEvent
{
    public partial class ChatMessageEventPublisher : ILogEventPublisher
    {
        public ChatMessageEventPublisher(EventHandler eventHandler, object? sender = null)
        {
            ArgumentNullException.ThrowIfNull(eventHandler, nameof(eventHandler));

            _eventHandler = eventHandler;
            _sender = sender;
        }

        private readonly object? _sender;

        private readonly EventHandler _eventHandler;

        public bool Match(MinecraftLog log)
        {
            return log.Thread == "Server thread" && (log.Message.StartsWith('<') || log.Message.StartsWith("[Not Secure] <")) && log.Message.Contains('>');
        }

        public bool TryParse(string message, [MaybeNullWhen(false)] out EventArgs result)
        {
            if (message.StartsWith("[Not Secure] "))
                message = message.Replace("[Not Secure] ", string.Empty);

            Match match = GetRegex().Match(message);

            if (match.Success)
            {
                ChatMessageInfo chatMessageInfo = new(match.Groups[1].Value, match.Groups[2].Value);
                result = new EventArgs<ChatMessageInfo>(chatMessageInfo);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public void TriggerEvent(EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e, nameof(e));

            _eventHandler.Invoke(_sender, e);
        }

        [GeneratedRegex(@"^<(\w+)> (.+)$")]
        private static partial Regex GetRegex();
    }
}
