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
    public class HybridMinecraftServer : MinecraftServer, IHybridInstance
    {

        public HybridMinecraftServer(string serverPath, string serverAddress, ushort serverPort, ushort rconPort, string rconPassword, ServerLaunchArguments launchArguments, ILoggerGetter? loggerGetter = null) : base(serverPath, serverAddress, serverPort, loggerGetter)
        {
            ArgumentException.ThrowIfNullOrEmpty(rconPassword, nameof(rconPassword));

            RconPort = rconPort;
            RconPassword = rconPassword;
            RCON = new(ServerAddress, RconPort, RconPassword);
            TwowayCommandSender = new(RCON);

            ServerProcess = new(ServerPathManager.Minecraft.FullName, launchArguments, loggerGetter);
            ServerConsole = new(ServerProcess.Process.StandardOutput, ServerProcess.Process.StandardInput, loggerGetter);
            OnewayCommandSender = new(ServerConsole);

            CommandSender = new(TwowayCommandSender, OnewayCommandSender);
        }

        public ushort RconPort { get; }

        public string RconPassword { get; }

        public RCON RCON { get; }

        public ServerProcess ServerProcess { get; }

        public ServerConsole ServerConsole { get; }

        public RconTwowayCommandSender TwowayCommandSender { get; }

        public ConsoleCommandSender OnewayCommandSender { get; }

        public override CommandSender CommandSender { get; }

        public override string InstanceKey => IHybridInstance.INSTANCE_KEY;

        protected override void Run()
        {
            LogFileListener.Start("LogFileListener Thread");
            ServerProcess.Start("ServerProcess Thread");
            ServerConsole.Start("ServerConsole Thread");
            RCON.ConnectAsync().Wait();

            Task.WaitAll(LogFileListener.WaitForStopAsync(), ServerProcess.WaitForStopAsync());
        }

        protected override void DisposeUnmanaged()
        {
            LogFileListener.Stop();
            ServerProcess.Stop();
            ServerConsole.Stop();
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
