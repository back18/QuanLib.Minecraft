using CoreRCON;
using CoreRCON.Parsers.Standard;
using QuanLib.Minecraft.Data;
using QuanLib.Minecraft.Files;
using QuanLib.Minecraft.Selectors;
using QuanLib.Minecraft.Snbt;
using QuanLib.Minecraft.Vectors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    //TODO 应该为抽象类，派生监听器模式和托管进程模式
    public class MinecraftServer
    {
        public MinecraftServer(string serverPath, string serverAddress)
        {
            if (string.IsNullOrEmpty(serverPath))
                throw new ArgumentException($"“{nameof(serverPath)}”不能为 null 或空。", nameof(serverPath));
            if (string.IsNullOrEmpty(serverAddress))
                throw new ArgumentException($"“{nameof(serverAddress)}”不能为 null 或空。", nameof(serverAddress));

            PathManager = new(serverPath);
            FileHelper = new(PathManager);

            Dictionary<string, string> properties = FileHelper.GetServerProperties();
            if (properties["enable-rcon"] != "true")
                throw new InvalidOperationException("需要在 server.properties 中将 enable-rcon 设置为 true");
            if (!ushort.TryParse(properties["server-port"], out var serverPort))
                throw new InvalidOperationException($"需要在 server.properties 中为 server-port 设置一个 {ushort.MinValue} 到 {ushort.MaxValue} 之间的有效端口");
            if (!ushort.TryParse(properties["rcon.port"], out var rconPort))
                throw new InvalidOperationException($"需要在 server.properties 中为 rcon.port 设置一个 {ushort.MinValue} 到 {ushort.MaxValue} 之间的有效端口");
            if (string.IsNullOrEmpty(properties["rcon.password"]))
                throw new InvalidOperationException($"需要在 server.properties 中为 rcon.password 设置一个非空密码");

            ServerAddress = serverAddress;
            ServerPort = serverPort;
            RconPort = rconPort;

            _ping = new();
            _rcon = new(IPAddress.Parse(serverAddress), rconPort, properties["rcon.password"]);
            _rconMax = new(serverAddress, rconPort, properties["rcon.password"]);
            CommandHelper = new(_rcon);
            CommandSender = _rconMax;
            ManagerMode = ServerManagerMode.NotConnected;

            LogParser.RconRunning += LogParser_RconRunning; //TODO
        }

        private TcpClient _ping;

        public readonly RCON _rcon;

        private readonly RconMax _rconMax;

        private ILogListener? _logListener;

        public ServerPathManager PathManager { get; }

        public ServerFileHelper FileHelper { get; }

        public ServerLogParser LogParser { get; }

        public ServerCommandHelper CommandHelper { get; }

        public ServerManagerMode ManagerMode { get; private set; }

        public ICommandSender CommandSender { get; private set; }

        public ServerLauncher? Launcher { get; private set; }

        public string ServerAddress { get; }

        public ushort ServerPort { get; }

        public ushort RconPort { get; }

        private void LogParser_RconRunning(ServerLogParser sender, QuanLib.Event.IPEndPointEventArgs e)
        {
            _rcon.ConnectAsync().Wait();
            if (ManagerMode == ServerManagerMode.Listener)
                _rconMax.Connect();
        }

        public bool Ping(out TimeSpan time)
        {
            lock (_ping)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    _ping.Connect(ServerAddress, ServerPort);
                }
                catch
                {
                    time = TimeSpan.Zero;
                    return false;
                }
                finally
                {
                    if (_ping.Connected)
                    {
                        Task.Run(() =>
                        {
                            lock (_ping)
                            {
                                _ping.Close();
                                _ping = new();
                            }
                        });
                    }
                }
                stopwatch.Stop();
                time = stopwatch.Elapsed;
                return true;
            }
        }

        public ServerLauncher CreatNewServerProcess(ServerLaunchArguments arguments)
        {
            Launcher = new(PathManager.MainDir, arguments);
            _logListener = Launcher;
            CommandSender = Launcher;
            ManagerMode = ServerManagerMode.ManagedProcess;
            return Launcher;
        }

        public void ConnectExistingServer()
        {
            LogFileListener logFileListener = new(PathManager.LatestLogFile);
            Task.Run(() => logFileListener.Start());
            _logListener = logFileListener;
            try
            {
                _rcon.ConnectAsync().Wait();
                _rconMax.Connect();
            }
            catch { }
            CommandSender = _rconMax;
            ManagerMode = ServerManagerMode.Listener;
        }
    }
}
