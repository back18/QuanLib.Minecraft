using QuanLib.Core;
using QuanLib.Minecraft.API;
using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.Instance.CommandSenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public class McapiMinecraftClient : MinecraftClient, IMcapiInstance
    {
        public McapiMinecraftClient(string clientPath, string mcapiAddress, ushort mcapiPort, string mcapiPassword, ILoggerGetter? loggerGetter = null) : base(clientPath, loggerGetter)
        {
            ArgumentException.ThrowIfNullOrEmpty(mcapiAddress, nameof(mcapiAddress));
            ArgumentException.ThrowIfNullOrEmpty(mcapiPassword, nameof(mcapiPassword));

            McapiAddress = IPAddress.TryParse(mcapiAddress, out var address) ? address : Dns.GetHostAddresses(mcapiAddress)[0];
            McapiPort = mcapiPort;
            McapiPassword = mcapiPassword;
            McapiClient = new(McapiAddress, McapiPort, loggerGetter);
            McapiCommandSender = new(McapiClient);
            CommandSender = new(McapiCommandSender, McapiCommandSender);
        }

        public IPAddress McapiAddress { get; }

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
            return NetworkUtil.TestTcpConnectivity(McapiAddress, McapiPort);
        }

        public override async Task<bool> TestConnectivityAsync()
        {
            return await NetworkUtil.TestTcpConnectivityAsync(McapiAddress, McapiPort);
        }
    }
}
