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
    public class HybridMinecraftServer : MinecraftServer, IHybridInstance
    {
        private const string LOG4J2_CONFIG = "log4j2-custom.xml";

        public HybridMinecraftServer(string serverPath,
            string serverAddress,
            ushort serverPort,
            ushort rconPort,
            string rconPassword,
            ServerLaunchArguments launchArguments,
            IList<string>? mclogRegexFilter = null,
            ILoggerGetter? loggerGetter = null)
            : base(serverPath, serverAddress, serverPort, loggerGetter)
        {
            ArgumentException.ThrowIfNullOrEmpty(rconPassword, nameof(rconPassword));
            if (!IsLocalServer)
                throw new NotSupportedException($"“{IHybridInstance.IDENTIFIER}”实例仅支持本地服务端");

            string? extraArguments;
            if (mclogRegexFilter is null || mclogRegexFilter.Count == 0)
            {
                extraArguments = null;
                _mclogRegexFilter = Array.Empty<string>();
            }
            else
            {
                extraArguments = "-Dlog4j.configurationFile=" + LOG4J2_CONFIG;
                _mclogRegexFilter = mclogRegexFilter.ToArray();
            }

            RconPort = rconPort;
            RconPassword = rconPassword;
            RCON = new(ServerAddress, RconPort, RconPassword);
            TwowayCommandSender = new(RCON);

            ServerProcess = new(serverPath, launchArguments, extraArguments, loggerGetter);
            ServerConsole = new(ServerProcess.Process, loggerGetter);
            OnewayCommandSender = new(ServerConsole);

            CommandSender = new(TwowayCommandSender, OnewayCommandSender);
            ConsoleLogListener = new(ServerConsole);
            LogAnalyzer = new(ConsoleLogListener);
        }

        private readonly string[] _mclogRegexFilter;

        public ushort RconPort { get; }

        public string RconPassword { get; }

        public RCON RCON { get; }

        public ServerProcess ServerProcess { get; }

        public ServerConsole ServerConsole { get; }

        public RconTwowayCommandSender TwowayCommandSender { get; }

        public ConsoleCommandSender OnewayCommandSender { get; }

        public override string Identifier => IHybridInstance.IDENTIFIER;

        public override CommandSender CommandSender { get; }

        public override ILogListener LogListener => ConsoleLogListener;

        public override bool IsSubprocess => true;

        public virtual ServerConsoleLogListener ConsoleLogListener { get; }

        public override LogAnalyzer LogAnalyzer { get; }

        protected override void Run()
        {
            if (_mclogRegexFilter.Length > 0)
            {
                Log4j2Configuration configuration = Log4j2Configuration.Load();

                foreach (string regex in _mclogRegexFilter)
                    configuration.AddRegexFilter(regex, MatchBehavior.DENY, MatchBehavior.NEUTRAL);

                configuration.Save(Path.Combine(MinecraftPath, LOG4J2_CONFIG));
            }

            ServerProcess.Start("ServerProcess Thread");
            ServerConsole.Start("ServerConsole Thread");

            TaskSemaphore semaphore = new();
            LogAnalyzer.RconRunning += (sender, e) => semaphore.Release();
            semaphore.Wait();

            RCON.ConnectAsync().Wait();

            Task.WaitAll(ServerProcess.WaitForStopAsync(), ServerConsole.WaitForStopAsync());
        }

        protected override void DisposeUnmanaged()
        {
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
