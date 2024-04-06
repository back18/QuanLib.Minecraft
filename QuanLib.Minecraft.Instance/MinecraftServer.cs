using QuanLib.Core;
using QuanLib.Minecraft.Directorys;
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
            ServerDirectory = new(serverPath);

            if (!File.Exists(ServerDirectory.ServerPropertiesFile))
                throw new FileNotFoundException("服务器配置文件不存在", ServerDirectory.ServerPropertiesFile);

            string text = File.ReadAllText(ServerDirectory.ServerPropertiesFile);
            Dictionary<string, string> dictionary = ServerProperties.Parse(text);
            ServerProperties = new(dictionary);
        }

        public override InstanceType InstanceType => InstanceType.Server;

        public override MinecraftDirectory MinecraftDirectory => ServerDirectory is null ? base.MinecraftDirectory : ServerDirectory;

        public virtual MinecraftServerDirectory ServerDirectory { get; }

        public IPAddress ServerAddress { get; }

        public ushort ServerPort { get; }

        public ServerProperties ServerProperties { get; }
    }
}
