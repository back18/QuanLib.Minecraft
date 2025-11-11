using CoreRCON;
using QuanLib.Minecraft.Instance.CommandSenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public interface IRconInstance
    {
        public const string IDENTIFIER = "RCON";

        public ushort RconPort { get; }

        public string RconPassword { get; }

        public RCON RCON { get; }

        public RconTwowayCommandSender TwowayCommandSender { get; }

        public RconOnewayCommandSender OnewayCommandSender { get; }
    }
}
