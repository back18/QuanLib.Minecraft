using QuanLib.Core;
using QuanLib.Minecraft.PathManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public abstract class MinecraftServer : MinecraftInstance
    {
        protected MinecraftServer(string serverPath, string serverAddress, ushort serverPort, ILoggerGetter? loggerGetter = null) : base(serverPath, loggerGetter)
        {
            ArgumentException.ThrowIfNullOrEmpty(serverAddress, nameof(serverAddress));

            ServerAddress = IPAddress.TryParse(serverAddress, out var address) ? address : Dns.GetHostAddresses(serverAddress)[0];
            ServerPort = serverPort;
            ServerPathManager = new(serverPath);

            ThrowHelper.FileNotFound(ServerPathManager.Minecraft_ServerProperties.FullName);

            string text = File.ReadAllText(ServerPathManager.Minecraft_ServerProperties.FullName);
            Dictionary<string, string> dictionary = ServerProperties.Parse(text);
            ServerProperties = new(dictionary);
        }

        public override InstanceType InstanceType => InstanceType.Server;

        public override MinecraftPathManager MinecraftPathManager => ServerPathManager is null ? base.MinecraftPathManager : ServerPathManager;

        public virtual ServerPathManager ServerPathManager { get; }

        public IPAddress ServerAddress { get; }

        public ushort ServerPort { get; }

        public ServerProperties ServerProperties { get; }
    }
}
