using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class PlayerLoginInfo
    {
        public PlayerLoginInfo(string name, IPAddress ip, ushort port, int id, EntityPos position)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            IP = ip ?? throw new ArgumentNullException(nameof(ip));
            Port = port;
            ID = id;
            Position = position;
        }

        public string Name { get; }

        public IPAddress IP { get; }

        public ushort Port { get; }

        public int ID { get; }

        public EntityPos Position { get; }

        public static bool TryParse(string s, [MaybeNullWhen(false)] out PlayerLoginInfo result)
        {
            if (string.IsNullOrEmpty(s))
                goto err;

            string pattern = @"(?<name>\w+)\[/(?<ip>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}):(?<port>\d+)\] logged in with entity id (?<id>\d+) at \((?<x>-?\d+\.\d+), (?<y>-?\d+\.\d+), (?<z>-?\d+\.\d+)\)";
            Match match = Regex.Match(s, pattern);
            if (match.Success)
            {
                string name = match.Groups["name"].Value;
                if (!IPAddress.TryParse(match.Groups["ip"].Value, out var ip) ||
                    !ushort.TryParse(match.Groups["port"].Value, out var port) ||
                    !int.TryParse(match.Groups["id"].Value, out var id) ||
                    !double.TryParse(match.Groups["x"].Value, out var x) ||
                    !double.TryParse(match.Groups["y"].Value, out var y) ||
                    !double.TryParse(match.Groups["z"].Value, out var z))
                    goto err;

                result = new(name, ip, port, id, new(x, y, z));
                return true;
            }

            err:
            result = null;
            return false;
        }
    }
}
