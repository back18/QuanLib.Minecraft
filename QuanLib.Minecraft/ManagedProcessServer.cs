using CoreRCON;
using QuanLib.Core.FileListeners;
using QuanLib.Minecraft.MinecraftLogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class ManagedProcessServer : MinecraftServer
    {
        public ManagedProcessServer(string serverPath, string serverAddress, ServerLaunchArguments launchArguments) : base(serverPath, serverAddress)
        {
            _rcon = new(IPAddress.Parse(ServerAddress), RconPort, RconPassword);

            CommandHelper = new(_rcon);
            Launcher = new(ServerDirectory.Directory, launchArguments);
            TextListener = Launcher;
            LogListener = Launcher;
            LogParser = new(Launcher);
            CommandHelper = new(_rcon);
            CommandSender = Launcher;

            _connected = false;
            _runing = false;
        }

        private readonly RCON _rcon;

        private bool _connected;

        private bool _runing;

        public override bool Connected => _connected;

        public override bool Runing => _runing;

        public ServerLauncher Launcher { get;}

        public override ITextListener TextListener { get; }

        public override ILogListener LogListener { get; }

        public override ServerLogParser LogParser { get; }

        public override ServerCommandHelper CommandHelper { get; }

        public override ICommandSender CommandSender { get; }

        public override MinecraftServerMode Mode => MinecraftServerMode.ManagedProcess;

        public override void WaitForConnected()
        {
            while (!_connected)
            {
                Thread.Sleep(10);
            }
        }

        public override void Start()
        {
            if (_runing)
                return;
            _runing = true;

            Task.Run(() =>
            {
                WaitForRconStartup();
                _rcon.ConnectAsync().Wait();
                _connected = true;
            });
            Launcher.Start();
        }

        public override void Stop()
        {
            _rcon.Dispose();
            _connected = false;
            Launcher.Stop();
            _runing = false;
        }
    }
}
