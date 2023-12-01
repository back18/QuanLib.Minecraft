using CoreRCON;
using log4net.Core;
using QuanLib.Core;
using QuanLib.Minecraft.API.Packet;
using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.Instance
{
    public class McapiMinecraftServer : MinecraftServer, IMcapiInstance
    {
        public McapiMinecraftServer(string serverPath, string serverAddress, ushort mcapiPort, string mcapiPassword, Func<Type, LogImpl> logger) : base(serverPath, serverAddress, logger)
        {
            ArgumentException.ThrowIfNullOrEmpty(mcapiPassword, nameof(mcapiPassword));

            McapiPort = mcapiPort;
            McapiPassword = mcapiPassword;
            McapiClient = new(ServerAddress, McapiPort, logger);
            McapiCommandSender = new(McapiClient);
            CommandSender = new(McapiCommandSender, McapiCommandSender);
        }

        public ushort McapiPort { get; }

        public string McapiPassword { get; }

        public McapiClient McapiClient { get; }

        public McapiCommandSender McapiCommandSender { get; }

        public override CommandSender CommandSender { get; }

        public override string InstanceKey => IMcapiInstance.INSTANCE_KEY;

        protected override void Run()
        {
            LogFileListener.Start("LogFileListener Thread");
            McapiClient.Start("McapiClient Thread");
            McapiClient.LoginAsync(McapiPassword).Wait();

            Task.WaitAll(LogFileListener.WaitForStopAsync(), McapiClient.WaitForStopAsync());
        }

        protected override void DisposeUnmanaged()
        {
            LogFileListener.Stop();
            McapiClient.Stop();
        }

        public override bool TestConnectivity()
        {
            Task<bool> server = NetworkUtil.TestTcpConnectivityAsync(ServerAddress, ServerPort);
            Task<bool> mcapi = NetworkUtil.TestTcpConnectivityAsync(ServerAddress, McapiPort);
            Task.WaitAll(server, mcapi);
            return server.Result && mcapi.Result;
        }

        public override async Task<bool> TestConnectivityAsync()
        {
            Task<bool> server = NetworkUtil.TestTcpConnectivityAsync(ServerAddress, ServerPort);
            Task<bool> mcapi = NetworkUtil.TestTcpConnectivityAsync(ServerAddress, McapiPort);
            return await server && await mcapi;
        }
    }
}
