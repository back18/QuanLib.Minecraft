using QuanLib.Core;
using QuanLib.Core.Events;
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
        public McapiMinecraftClient(string clientPath, string mcapiAddress, ushort mcapiPort, string mcapiPassword, ILoggerProvider? loggerProvider = null) : base(clientPath, loggerProvider)
        {
            ArgumentException.ThrowIfNullOrEmpty(mcapiAddress, nameof(mcapiAddress));
            ArgumentException.ThrowIfNullOrEmpty(mcapiPassword, nameof(mcapiPassword));

            McapiAddress = ParseIPAddress(mcapiAddress);
            if (!McapiAddress.Equals(IPAddress.Loopback))
                throw new NotSupportedException($"“{IMcapiInstance.IDENTIFIER}”客户端实例仅支持本地地址");

            McapiPort = mcapiPort;
            McapiPassword = mcapiPassword;
            McapiClient = new(McapiAddress, McapiPort, loggerProvider);
            McapiClient.SetDefaultThreadName("McapiClient Thread");
            AddSubtask(McapiClient);

            McapiCommandSender = new(McapiClient);
            CommandSender = new(McapiCommandSender, McapiCommandSender);

            FileInfo file = MinecraftPathManager.Minecraft_Logs_LatestLog;

            LogFileListener = new(file.FullName, loggerProvider: loggerProvider);
            LogFileListener.SetDefaultThreadName("LogFileListener Thread");
            AddSubtask(LogFileListener);

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

        protected override void OnSubtaskStopped(MultitaskRunnable sender, EventArgs<IRunnable> e)
        {
            base.OnSubtaskStopped(sender, e);

            if (!IsRunning)
                return;

            if (e.Argument == McapiClient)
            {
                Logger?.Error("MCAPI意外断开连接，Minecraft实例即将终止");
                IsRunning = false;
            }
        }

        protected override void Run()
        {
            (bool IsSuccessful, string Message) = McapiClient.LoginAsync(McapiPassword).ConfigureAwait(false).GetAwaiter().GetResult();
            if (!IsSuccessful)
                throw new InvalidOperationException("MCAPI登录失败：" + (string.IsNullOrEmpty(Message) ? "未知原因" : Message));

            WaitAllSubtask();
        }

        public override bool TestConnectivity()
        {
            return NetworkUtil.TcpListenerIsActive(McapiPort);
        }

        public override Task<bool> TestConnectivityAsync()
        {
            return Task.Run(() => NetworkUtil.TcpListenerIsActive(McapiPort));
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
