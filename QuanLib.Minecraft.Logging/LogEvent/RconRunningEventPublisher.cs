using QuanLib.Core.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Logging.LogEvent
{
    public partial class RconRunningEventPublisher : ILogEventPublisher
    {
        public RconRunningEventPublisher(EventHandler eventHandler, object? sender = null)
        {
            ArgumentNullException.ThrowIfNull(eventHandler, nameof(eventHandler));

            _eventHandler = eventHandler;
            _sender = sender;
        }

        private readonly object? _sender;

        private readonly EventHandler _eventHandler;

        public bool Match(MinecraftLog log)
        {
            return log.Message.StartsWith("RCON running");
        }

        public bool TryParse(string message, [MaybeNullWhen(false)] out EventArgs result)
        {
            Match match = GetRegex().Match(message);

            if (match.Success)
            {
                string value = match.Groups[1].Value;
                if (IPEndPoint.TryParse(value, out var iPEndPoint))
                {
                    result = new EventArgs<IPEndPoint>(iPEndPoint);
                    return true;
                }
            }

            result = null;
            return false;
        }

        public void TriggerEvent(EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e, nameof(e));

            _eventHandler.Invoke(_sender, e);
        }

        [GeneratedRegex(@"RCON running on (\d+\.\d+\.\d+\.\d+:\d+)")]
        private static partial Regex GetRegex();
    }
}
