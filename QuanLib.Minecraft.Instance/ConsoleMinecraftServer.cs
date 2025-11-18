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
    public class ConsoleMinecraftServer : MinecraftServer, IConsoleInstance
    {
        private const string LOG4J2_CONFIG = "log4j2-custom.xml";

        public ConsoleMinecraftServer(
            string serverPath,
            string serverAddress,
            ushort serverPort,
            ServerLaunchArguments launchArguments,
            IList<string>? mclogRegexFilter = null,
            ILoggerGetter? loggerGetter = null)
            : base(serverPath, serverAddress, serverPort, loggerGetter)
        {
            if (!IsLocalServer)
                throw new NotSupportedException($"“{IConsoleInstance.IDENTIFIER}”实例仅支持本地服务端");

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

            ServerProcess = new(serverPath, launchArguments, extraArguments, loggerGetter);
            ServerProcess.SetDefaultThreadName("ServerProcess Thread");
            AddSubtask(ServerProcess);

            ServerConsole = new(ServerProcess.Process, loggerGetter);
            ServerConsole.SetDefaultThreadName("ServerConsole Thread");
            AddSubtask(ServerConsole);

            ServerProcess.Started += ServerProcess_Started;

            ConsoleCommandSender = new(ServerConsole);
            CommandSender = new(ConsoleCommandSender, ConsoleCommandSender);
            ConsoleLogListener = new(ServerConsole);
            LogAnalyzer = new(ConsoleLogListener);
        }

        private readonly string[] _mclogRegexFilter;

        public ServerProcess ServerProcess { get; }

        public ServerConsole ServerConsole { get; }

        public ConsoleCommandSender ConsoleCommandSender { get; }

        public override string Identifier => IConsoleInstance.IDENTIFIER;

        public override bool IsSubprocess => true;

        public override CommandSender CommandSender { get; }

        public override ILogListener LogListener => ConsoleLogListener;

        public virtual ServerConsoleLogListener ConsoleLogListener { get; }

        public override LogAnalyzer LogAnalyzer { get; }

        protected override void Run()
        {
            WaitAllSubtask();
        }

        private void ServerProcess_Started(IRunnable sender, EventArgs e)
        {
            if (_mclogRegexFilter.Length > 0)
            {
                Log4j2Configuration configuration = Log4j2Configuration.Load();

                foreach (string regex in _mclogRegexFilter)
                    configuration.AddRegexFilter(regex, MatchBehavior.DENY, MatchBehavior.NEUTRAL);

                configuration.Save(Path.Combine(MinecraftPath, LOG4J2_CONFIG));
            }
        }
    }
}
