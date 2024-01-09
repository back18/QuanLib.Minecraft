using CoreRCON;
using QuanLib.Core;
using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.Instance.CommandSenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public class RconMinecraftServer : MinecraftServer, IRconInstance
    {
        public RconMinecraftServer(string serverPath, string serverAddress, ILogbuilder? logbuilder = null) : base(serverPath, serverAddress, logbuilder)
        {
            if (!ServerProperties.EnableRcon)
                throw new InvalidOperationException($"需要在 server.properties 中将 {ServerProperties.ENABLE_RCON} 设置为 true");
            if (ServerProperties.RconPort < ushort.MinValue || ServerProperties.RconPort > ushort.MaxValue)
                throw new InvalidOperationException($"需要在 server.properties 中为 {ServerProperties.RCON_PORT} 设置一个 {ushort.MinValue} 到 {ushort.MaxValue} 之间的有效端口");
            if (string.IsNullOrEmpty(ServerProperties.RconPassword))
                throw new InvalidOperationException($"需要在 server.properties 中为 {ServerProperties.RCON_PASSWORD} 设置一个非空密码");

            RconPort = (ushort)ServerProperties.RconPort;
            RconPassword = ServerProperties.RconPassword;
            RCON = new(ServerAddress, RconPort, RconPassword);
            TwowayCommandSender = new(RCON);
            OnewayCommandSender = new(ServerAddress, RconPort, ServerProperties.RconPassword, logbuilder: logbuilder);
            CommandSender = new(TwowayCommandSender, OnewayCommandSender);
        }

        public ushort RconPort { get; }

        public string RconPassword { get; }

        public RCON RCON { get; }

        public RconTwowayCommandSender TwowayCommandSender { get; }

        public RconOnewayCommandSender OnewayCommandSender { get; }

        public override CommandSender CommandSender { get; }

        public override string InstanceKey => IRconInstance.INSTANCE_KEY;

        protected override void Run()
        {
            LogFileListener.Start("LogFileListener Thread");
            OnewayCommandSender.Start("RconOnewayCommandSender Thread");
            RCON.ConnectAsync().Wait();

            Task.WaitAll(LogFileListener.WaitForStopAsync(), OnewayCommandSender.WaitForStopAsync());
        }

        protected override void DisposeUnmanaged()
        {
            LogFileListener.Stop();
            OnewayCommandSender.Stop();
            RCON.Dispose();
        }

        public override bool TestConnectivity()
        {
            Task<bool> server = NetworkUtil.TestTcpConnectivityAsync(ServerAddress, ServerPort);
            Task<bool> rcon = NetworkUtil.TestTcpConnectivityAsync(ServerAddress, RconPort);
            Task.WaitAll(server, rcon);
            return server.Result && rcon.Result;
        }

        public override async Task<bool> TestConnectivityAsync()
        {
            Task<bool> server = NetworkUtil.TestTcpConnectivityAsync(ServerAddress, ServerPort);
            Task<bool> rcon = NetworkUtil.TestTcpConnectivityAsync(ServerAddress, RconPort);
            return await server && await rcon;
        }
    }
}
