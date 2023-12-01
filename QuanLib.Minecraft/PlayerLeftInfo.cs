using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class PlayerLeftInfo
    {
        public PlayerLeftInfo(string name, string reason)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            ArgumentNullException.ThrowIfNull(reason, nameof(reason));

            Name = name;
            Reason = reason;
        }

        public string Name { get; }

        public string Reason { get; }
    }
}
