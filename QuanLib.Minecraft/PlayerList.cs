using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class PlayerList
    {
        public PlayerList(int onlinePlayers, int maxPlayers, IList<string> list)
        {
            OnlinePlayers = onlinePlayers;
            MaxPlayers = maxPlayers;
            List = new(list);
        }

        public static readonly PlayerList Empty = new(0, 0, Array.Empty<string>());

        public int OnlinePlayers { get; }

        public int MaxPlayers { get; }

        public ReadOnlyCollection<string> List { get; }
    }
}
