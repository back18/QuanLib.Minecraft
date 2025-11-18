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
using System.Net.NetworkInformation;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public class McapiMinecraftServer : MinecraftServer, IMcapiInstance
    {
        public McapiMinecraftServer(string serverPath, string serverAddress, ushort serverPort, ushort mcapiPort, string mcapiPassword, ILoggerGetter? loggerGetter = null) : base(serverPath, serverAddress, serverPort, loggerGetter)
        {
            ArgumentException.ThrowIfNullOrEmpty(mcapiPassword, nameof(mcapiPassword));

            McapiPort = mcapiPort;
            McapiPassword = mcapiPassword;
            McapiClient = new(ServerAddress, McapiPort, loggerGetter);
            McapiClient.SetDefaultThreadName("McapiClient Thread");
            AddSubtask(McapiClient);

            McapiCommandSender = new(McapiClient);
            CommandSender = new(McapiCommandSender, McapiCommandSender);

            if (IsLocalServer)
            {
                FileInfo file = MinecraftPathManager.Minecraft_Logs_LatestLog;

                _logFileListener = new(file.FullName, loggerGetter: loggerGetter);
                _logFileListener.SetDefaultThreadName("LogFileListener Thread");
                AddSubtask(_logFileListener);

                _logAnalyzer = new(_logFileListener);
            }
            else
            {
                _logFileListener = null;
                _logAnalyzer = null;
            }
        }

        private readonly PollingLogFileListener? _logFileListener;

        private readonly LogAnalyzer? _logAnalyzer;

        public ushort McapiPort { get; }

        public string McapiPassword { get; }

        public McapiClient McapiClient { get; }

        public McapiCommandSender McapiCommandSender { get; }

        public override string Identifier => IMcapiInstance.IDENTIFIER;

        public override bool IsSubprocess => false;

        public override CommandSender CommandSender { get; }

        public override ILogListener LogListener => LogFileListener;

        public virtual PollingLogFileListener LogFileListener => _logFileListener ?? throw new NotSupportedException(MESSAGE_REMOTE_SERVER_NOT_SUPPORTED);

        public override LogAnalyzer LogAnalyzer => _logAnalyzer ?? throw new NotSupportedException(MESSAGE_REMOTE_SERVER_NOT_SUPPORTED);

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
            if (IsLocalServer)
                return TestLocalConnectivity();

            Task<bool> server = NetworkUtil.TestTcpConnectivityAsync(ServerAddress, ServerPort);
            Task<bool> mcapi = NetworkUtil.TestTcpConnectivityAsync(ServerAddress, McapiPort);

            return server.ConfigureAwait(false).GetAwaiter().GetResult() && mcapi.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public override async Task<bool> TestConnectivityAsync()
        {
            if (IsLocalServer)
                return TestLocalConnectivity();

            Task<bool> server = NetworkUtil.TestTcpConnectivityAsync(ServerAddress, ServerPort);
            Task<bool> mcapi = NetworkUtil.TestTcpConnectivityAsync(ServerAddress, McapiPort);
            return await server.ConfigureAwait(false) && await mcapi.ConfigureAwait(false);
        }

        private bool TestLocalConnectivity()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] listeners = properties.GetActiveTcpListeners();
            return listeners.Any(s => s.Port == ServerPort) && listeners.Any(s => s.Port == McapiPort);
        }
    }
}
