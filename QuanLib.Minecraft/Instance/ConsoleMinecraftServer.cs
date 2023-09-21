using QuanLib.Core;
using QuanLib.Minecraft.Command.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public class ConsoleMinecraftServer : MinecraftServer, IConsoleInstance
    {
        public ConsoleMinecraftServer(string serverPath, string serverAddress, ServerLaunchArguments launchArguments) : base(serverPath, serverAddress)
        {
            ServerProcess = new(ServerDirectory.FullPath, launchArguments);
            ServerConsole = new(ServerProcess.Process.StandardOutput, ServerProcess.Process.StandardInput);
            ConsoleCommandSender = new(ServerConsole);
            CommandSender = new(ConsoleCommandSender, ConsoleCommandSender);
        }

        public ServerProcess ServerProcess { get; }

        public ServerConsole ServerConsole { get; }

        public ConsoleCommandSender ConsoleCommandSender { get; }

        public override CommandSender CommandSender { get; }

        public override string InstanceKey => IConsoleInstance.INSTANCE_KEY;

        protected override void Run()
        {
            LogFileListener.Start();
            ServerProcess.Start();
            ServerConsole.Start();

            Task.WaitAll(LogFileListener.WaitForStopAsync(), ServerProcess.WaitForStopAsync());
        }

        protected override void DisposeUnmanaged()
        {
            LogFileListener.Stop();
            ServerProcess.Stop();
            ServerConsole.Stop();
        }

        public override bool TestConnection()
        {
            return NetworkUtil.TestTcpConnection(ServerAddress, ServerPort);
        }
    }
}
