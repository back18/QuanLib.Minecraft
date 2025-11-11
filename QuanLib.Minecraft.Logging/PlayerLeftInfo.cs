using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Logging
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

        public override string ToString()
        {
            return $"{Name} lost connection: {Reason}";
        }
    }
}
