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
        public RconMinecraftServer(string serverPath, string serverAddress, ushort serverPort, ushort rconPort, string rconPassword, ILoggerGetter? loggerGetter = null) : base(serverPath, serverAddress, serverPort, loggerGetter)
        {
            ArgumentException.ThrowIfNullOrEmpty(rconPassword, nameof(rconPassword));

            RconPort = rconPort;
            RconPassword = rconPassword;
            RCON = new(ServerAddress, RconPort, RconPassword);
            TwowayCommandSender = new(RCON);
            OnewayCommandSender = new(ServerAddress, RconPort, ServerProperties.RconPassword, loggerGetter: loggerGetter);
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
