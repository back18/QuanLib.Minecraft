using QuanLib.Core;
using QuanLib.Minecraft.PathManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public abstract class MinecraftServer : MinecraftInstance
    {
        protected const string MESSAGE_REMOTE_SERVER_NOT_SUPPORTED = "远程服务器不支持访问此属性";

        protected MinecraftServer(string serverPath, string serverAddress, ushort serverPort, ILoggerGetter? loggerGetter = null) : base(serverPath, loggerGetter)
        {
            ArgumentException.ThrowIfNullOrEmpty(serverAddress, nameof(serverAddress));

            ServerAddress = ParseIPAddress(serverAddress);
            ServerPort = serverPort;

            if (IsLocalServer)
            {
                _serverPathManager = new(serverPath);
                FileInfo file = _serverPathManager.Minecraft_ServerProperties;
                ThrowHelper.FileNotFound(file.FullName);

                string text = File.ReadAllText(_serverPathManager.Minecraft_ServerProperties.FullName);
                Dictionary<string, string> dictionary = ServerProperties.Parse(text);
                _serverProperties = new(dictionary);
            }
            else
            {
                _serverPathManager = null;
                _serverProperties = null;
            }
        }

        private readonly ServerPathManager? _serverPathManager;

        private readonly ServerProperties? _serverProperties;

        public override bool IsClient => false;

        public virtual bool IsLocalServer =>  ServerAddress.Equals(IPAddress.Loopback);

        public override MinecraftPathManager MinecraftPathManager => ServerPathManager;

        public virtual ServerPathManager ServerPathManager => _serverPathManager ?? throw new NotSupportedException(MESSAGE_REMOTE_SERVER_NOT_SUPPORTED);

        public IPAddress ServerAddress { get; }

        public ushort ServerPort { get; }

        public ServerProperties ServerProperties => _serverProperties ?? throw new NotSupportedException(MESSAGE_REMOTE_SERVER_NOT_SUPPORTED);

        public override bool TestConnectivity()
        {
            if (IsLocalServer)
                return NetworkUtil.TcpListenerIsActive(ServerPort);

            return NetworkUtil.TestTcpConnectivity(ServerAddress, ServerPort);
        }

        public override async Task<bool> TestConnectivityAsync()
        {
            if (IsLocalServer)
                return NetworkUtil.TcpListenerIsActive(ServerPort);

            return await NetworkUtil.TestTcpConnectivityAsync(ServerAddress, ServerPort);
        }

        private static IPAddress ParseIPAddress(string address)
        {
            ArgumentException.ThrowIfNullOrEmpty(address, nameof(address));

            if (IPAddress.TryParse(address, out var iPAddress))
                return iPAddress;

            IPAddress[] iPAddresses = Dns.GetHostAddresses(address);

            if (iPAddresses.Length == 0)
                throw new ArgumentException($"无法将“{address}”解析为IP地址或域名", nameof(address));

            IPAddress[] iPv4Addresses = iPAddresses
                .Where(addr => addr.AddressFamily == AddressFamily.InterNetwork)
                .ToArray();

            if (iPv4Addresses.Length == 0)
                throw new ArgumentException($"无法将“{address}”映射为IPv4地址", nameof(address));

            return iPv4Addresses[0];
        }
    }
}
