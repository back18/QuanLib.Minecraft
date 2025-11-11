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
    public partial class PlayerLeftEventPublisher : ILogEventPublisher
    {
        public PlayerLeftEventPublisher(EventHandler eventHandler, object? sender = null)
        {
            ArgumentNullException.ThrowIfNull(eventHandler, nameof(eventHandler));

            _eventHandler = eventHandler;
            _sender = sender;
        }

        private readonly object? _sender;

        private readonly EventHandler _eventHandler;

        public bool Match(MinecraftLog log)
        {
            return log.Thread == "Server thread" && log.Message.Contains("lost connection");
        }

        public bool TryParse(string message, [MaybeNullWhen(false)] out EventArgs result)
        {
            Match match = GetRegex().Match(message);

            if (match.Success)
            {
                PlayerLeftInfo playerLeftInfo = new(match.Groups["name"].Value, match.Groups["reason"].Value);
                result = new EventArgs<PlayerLeftInfo>(playerLeftInfo);
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

        [GeneratedRegex(@"^(?<name>\w+) lost connection: (?<reason>.+)$")]
        private static partial Regex GetRegex();
    }
}
