using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class PlayerList
    {
        public PlayerList(int onlinePlayers, int maxPlayers, IReadOnlyList<string> players)
        {
            OnlinePlayers = onlinePlayers;
            MaxPlayers = maxPlayers;
            Players = players;
        }

        public PlayerList(int onlinePlayers, int maxPlayers, IEnumerable<string> players) :
            this(onlinePlayers, maxPlayers, players?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(players))) { }

        public int OnlinePlayers { get; }

        public int MaxPlayers { get; }

        public IReadOnlyList<string> Players { get; }
    }
}
