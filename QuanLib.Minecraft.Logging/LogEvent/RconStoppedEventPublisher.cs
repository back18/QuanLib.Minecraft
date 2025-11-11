using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Logging.LogEvent
{
    public class RconStoppedEventPublisher : ILogEventPublisher
    {
        private const string MESSAGE = "Thread RCON Listener stopped";

        public RconStoppedEventPublisher(EventHandler eventHandler, object? sender = null)
        {
            ArgumentNullException.ThrowIfNull(eventHandler, nameof(eventHandler));

            _eventHandler = eventHandler;
            _sender = sender;
        }

        private readonly object? _sender;

        private readonly EventHandler _eventHandler;

        public bool Match(MinecraftLog log)
        {
            return log.Message.Length == MESSAGE.Length;
        }

        public bool TryParse(string message, [MaybeNullWhen(false)] out EventArgs result)
        {
            if (message == MESSAGE)
            {
                result = EventArgs.Empty;
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
    }
}
