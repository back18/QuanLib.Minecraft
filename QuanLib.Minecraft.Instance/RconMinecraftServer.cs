using CoreRCON;
using QuanLib.Core;
using QuanLib.Core.Events;
using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.Instance.CommandSenders;
using QuanLib.Minecraft.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
            OnewayCommandSender.SetDefaultThreadName("RconOnewayCommandSender Thread");
            AddSubtask(OnewayCommandSender);

            CommandSender = new(TwowayCommandSender, OnewayCommandSender);

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

        public ushort RconPort { get; }

        public string RconPassword { get; }

        public RCON RCON { get; }

        public RconTwowayCommandSender TwowayCommandSender { get; }

        public RconOnewayCommandSender OnewayCommandSender { get; }

        public override string Identifier => IRconInstance.IDENTIFIER;

        public override bool IsSubprocess => false;

        public override CommandSender CommandSender { get; }

        public override ILogListener LogListener => LogFileListener;

        public virtual PollingLogFileListener LogFileListener => _logFileListener ?? throw new NotSupportedException(MESSAGE_REMOTE_SERVER_NOT_SUPPORTED);

        public override LogAnalyzer LogAnalyzer => _logAnalyzer ?? throw new NotSupportedException(MESSAGE_REMOTE_SERVER_NOT_SUPPORTED);

        protected override void OnSubtaskStopped(MultitaskRunnable sender, EventArgs<IRunnable> e)
        {
            base.OnSubtaskStopped(sender, e);

            if (e.Argument == OnewayCommandSender)
            {
                Logger?.Error("RCON意外断开连接，Minecraft实例即将终止");
                IsRunning = false;
            }
        }

        protected override void Run()
        {
            RCON.ConnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            WaitAllSubtask();
        }

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();
            RCON.Dispose();
        }

        public override bool TestConnectivity()
        {
            if (IsLocalServer)
                return TestLocalConnectivity();

            Task<bool> server = NetworkUtil.TestTcpConnectivityAsync(ServerAddress, ServerPort);
            Task<bool> rcon = NetworkUtil.TestTcpConnectivityAsync(ServerAddress, RconPort);

            return server.ConfigureAwait(false).GetAwaiter().GetResult() && rcon.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public override async Task<bool> TestConnectivityAsync()
        {
            if (IsLocalServer)
                return TestLocalConnectivity();

            Task<bool> server = NetworkUtil.TestTcpConnectivityAsync(ServerAddress, ServerPort);
            Task<bool> rcon = NetworkUtil.TestTcpConnectivityAsync(ServerAddress, RconPort);
            return await server.ConfigureAwait(false) && await rcon.ConfigureAwait(false);
        }

        private bool TestLocalConnectivity()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] listeners = properties.GetActiveTcpListeners();
            return listeners.Any(s => s.Port == ServerPort) && listeners.Any(s => s.Port == RconPort);
        }
    }
}
