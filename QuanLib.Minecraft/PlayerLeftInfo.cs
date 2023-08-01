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
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        }

        public string Name { get; }

        public string Reason { get; }
    }
}
