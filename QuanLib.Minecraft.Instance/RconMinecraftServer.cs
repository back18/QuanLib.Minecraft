using CoreRCON;
using QuanLib.Core;
using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.Instance.CommandSenders;
using QuanLib.Minecraft.Logging;
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

            if (IsLocalServer)
            {
                FileInfo file = MinecraftPathManager.Minecraft_Logs_LatestLog;
                _logFileListener = new(file.FullName);
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

        protected override void Run()
        {
            _logFileListener?.Start("LogFileListener Thread");
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
