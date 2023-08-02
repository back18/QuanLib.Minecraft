using CoreRCON;
using QuanLib.Minecraft.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class RconConnectServer : MinecraftServer
    {
        public RconConnectServer(string serverPath, string serverAddress) : base(serverPath, serverAddress)
        {
            _rcon = new(IPAddress.Parse(ServerAddress), RconPort, RconPassword);

            LogFileListener = new(PathManager.LatestLogFile);
            RconMax = new(ServerAddress, RconPort, RconPassword);
            LogParser = new(LogFileListener);
            CommandHelper = new(_rcon);
            CommandSender = RconMax;

            _connected = false;
            _runing = false;
        }

        private readonly RCON _rcon;

        private bool _connected;

        private bool _runing;

        public override bool Connected => _connected;

        public override bool Runing => _runing;

        public LogFileListener LogFileListener { get; }

        public RconMax RconMax { get; }

        public override ServerLogParser LogParser { get; }

        public override ServerCommandHelper CommandHelper { get; }

        public override ICommandSender CommandSender { get; }

        public override MinecraftServerMode Mode => MinecraftServerMode.RconConnect;

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
                RconMax.Connect();
                _rcon.ConnectAsync().Wait();
                _connected = true;
            });
            LogFileListener.Start();
        }

        public override void Stop()
        {
            RconMax.Dispose();
            _rcon.Dispose();
            _connected = false;
            LogFileListener.Stop();
            _runing = false;
        }
    }
}
