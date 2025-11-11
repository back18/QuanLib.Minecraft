using QuanLib.Core;
using QuanLib.Minecraft.API;
using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.Instance.CommandSenders;
using QuanLib.Minecraft.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

            McapiAddress = ParseIPAddress(mcapiAddress);
            McapiPort = mcapiPort;
            McapiPassword = mcapiPassword;
            McapiClient = new(McapiAddress, McapiPort, loggerGetter);
            McapiCommandSender = new(McapiClient);
            CommandSender = new(McapiCommandSender, McapiCommandSender);

            FileInfo file = MinecraftPathManager.Minecraft_Logs_LatestLog;
            LogFileListener = new(file.FullName);
            LogAnalyzer = new(LogFileListener);
        }

        public IPAddress McapiAddress { get; }

        public ushort McapiPort { get; }

        public string McapiPassword { get; }

        public McapiClient McapiClient { get; }

        public McapiCommandSender McapiCommandSender { get; }

        public override string Identifier => IMcapiInstance.IDENTIFIER;

        public override CommandSender CommandSender { get; }

        public override ILogListener LogListener => LogFileListener;

        public override bool IsSubprocess => false;

        public virtual PollingLogFileListener LogFileListener { get; }

        public override LogAnalyzer LogAnalyzer { get; }

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
