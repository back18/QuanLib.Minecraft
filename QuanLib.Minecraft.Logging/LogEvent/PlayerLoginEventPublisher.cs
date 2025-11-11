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
    public partial class PlayerLoginEventPublisher : ILogEventPublisher
    {
        public PlayerLoginEventPublisher(EventHandler eventHandler, object? sender = null)
        {
            ArgumentNullException.ThrowIfNull(eventHandler, nameof(eventHandler));

            _eventHandler = eventHandler;
            _sender = sender;
        }

        private readonly object? _sender;

        private readonly EventHandler _eventHandler;

        public bool Match(MinecraftLog log)
        {
            return log.Thread == "Server thread" && log.Message.Contains("logged in with entity");
        }

        public bool TryParse(string message, [MaybeNullWhen(false)] out EventArgs result)
        {
            Match match = GetRegex().Match(message);

            if (match.Success)
            {
                string name = match.Groups["name"].Value;
                string address = match.Groups["address"].Value;
                string id = match.Groups["id"].Value;
                string x = match.Groups["x"].Value;
                string y = match.Groups["y"].Value;
                string z = match.Groups["z"].Value;

                if (int.TryParse(id, out var intId) &&
                    double.TryParse(x, out var doubleX) &&
                    double.TryParse(y, out var doubleY) &&
                    double.TryParse(z, out var doubleZ))
                {
                    PlayerLoginInfo playerLoginInfo = new(name, address, intId, new(doubleX, doubleY, doubleZ));
                    result = new EventArgs<PlayerLoginInfo>(playerLoginInfo);
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

        [GeneratedRegex(@"^(?<name>.*?)\[(?<address>.*?)\] logged in with entity id (?<id>\d+) at \((?<x>-?\d+\.\d+), (?<y>-?\d+\.\d+), (?<z>-?\d+\.\d+)\)$")]
        private static partial Regex GetRegex();
    }
}
