﻿using QuanLib.Core;
using QuanLib.Minecraft.Command.Senders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public class ConsoleMinecraftServer : MinecraftServer, IConsoleInstance
    {
        public ConsoleMinecraftServer(string serverPath, string serverAddress, ServerLaunchArguments launchArguments, ILogbuilder? logbuilder = null) : base(serverPath, serverAddress, logbuilder)
        {
            ServerProcess = new(ServerDirectory.FullPath, launchArguments, logbuilder);
            ServerConsole = new(ServerProcess.Process.StandardOutput, ServerProcess.Process.StandardInput, logbuilder);
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
            LogFileListener.Start("LogFileListener Thread");
            ServerProcess.Start("ServerProcess Thread");
            ServerConsole.Start("ServerConsole Thread");

            Task.WaitAll(LogFileListener.WaitForStopAsync(), ServerProcess.WaitForStopAsync());
        }

        protected override void DisposeUnmanaged()
        {
            LogFileListener.Stop();
            ServerProcess.Stop();
            ServerConsole.Stop();
        }

        public override bool TestConnectivity()
        {
            return NetworkUtil.TestTcpConnectivity(ServerAddress, ServerPort);
        }

        public override async Task<bool> TestConnectivityAsync()
        {
            return await NetworkUtil.TestTcpConnectivityAsync(ServerAddress, ServerPort);
        }
    }
}
