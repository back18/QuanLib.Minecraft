using QuanLib.Core;
using QuanLib.Minecraft.API;
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
    public class McapiMinecraftServer : MinecraftServer, IMcapiInstance
    {
        public McapiMinecraftServer(string serverPath, string serverAddress, ushort serverPort, ushort mcapiPort, string mcapiPassword, ILoggerGetter? loggerGetter = null) : base(serverPath, serverAddress, serverPort, loggerGetter)
        {
            ArgumentException.ThrowIfNullOrEmpty(mcapiPassword, nameof(mcapiPassword));

            McapiPort = mcapiPort;
            McapiPassword = mcapiPassword;
            McapiClient = new(ServerAddress, McapiPort, loggerGetter);
            McapiCommandSender = new(McapiClient);
            CommandSender = new(McapiCommandSender, McapiCommandSender);

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
