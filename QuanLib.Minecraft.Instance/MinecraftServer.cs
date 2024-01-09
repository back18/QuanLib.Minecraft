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
        protected MinecraftServer(string serverPath, string serverAddress, ILogbuilder? logbuilder = null) : base(serverPath, logbuilder)
        {
            ArgumentException.ThrowIfNullOrEmpty(serverAddress, nameof(serverAddress));

            ServerAddress = IPAddress.Parse(serverAddress);
            ServerDirectory = new(serverPath);

            if (!File.Exists(ServerDirectory.ServerPropertiesFile))
                throw new FileNotFoundException("服务器配置文件不存在", ServerDirectory.ServerPropertiesFile);
            string text = File.ReadAllText(ServerDirectory.ServerPropertiesFile);
            Dictionary<string, string> dictionary = ServerProperties.Parse(text);
            ServerProperties = new(dictionary);

            if (ServerProperties.ServerPort < ushort.MinValue || ServerProperties.ServerPort > ushort.MaxValue)
                throw new InvalidOperationException($"需要在 server.properties 中为 {ServerProperties.SERVER_PORT} 设置一个 {ushort.MinValue} 到 {ushort.MaxValue} 之间的有效端口");

            ServerPort = (ushort)ServerProperties.ServerPort;
        }

        public override InstanceType InstanceType => InstanceType.Server;

        public override MinecraftDirectory MinecraftDirectory => ServerDirectory is null ? base.MinecraftDirectory : ServerDirectory;

        public virtual MinecraftServerDirectory ServerDirectory { get; }

        public IPAddress ServerAddress { get; }

        public ushort ServerPort { get; }

        public ServerProperties ServerProperties { get; }
    }
}
