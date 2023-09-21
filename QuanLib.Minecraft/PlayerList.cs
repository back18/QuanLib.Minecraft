using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class PlayerList
    {
        public PlayerList(int onlinePlayers, int maxPlayers, IReadOnlyList<string> list)
        {
            OnlinePlayers = onlinePlayers;
            MaxPlayers = maxPlayers;
            List = list ?? throw new ArgumentNullException(nameof(list));
        }

        public static readonly PlayerList Empty = new(0, 0, Array.Empty<string>());

        public int OnlinePlayers { get; }

        public int MaxPlayers { get; }

        public IReadOnlyList<string> List { get; }
    }
}
