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
    public abstract class MinecraftServer
    {
        protected MinecraftServer(string serverPath, string serverAddress)
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

            OnWriteLog += (obj) => { };
            OnPlayerJoined += MinecraftServer_OnPlayerJoined;
            OnPlayerLeft += MinecraftServer_OnPlayerLeft;
            OnRconRunning += MinecraftServer_OnRconRunning;

            _loginInfos = new();
        }

        private TcpClient _ping;

        public readonly RCON _rcon;

        private readonly RconMax _rconMax;

        private ILogListener? _logListener;

        private readonly Dictionary<string, PlayerLoginInfo> _loginInfos;

        public IReadOnlyDictionary<string, PlayerLoginInfo> LoginInfos => _loginInfos;

        public ServerPathManager PathManager { get; }

        public ServerFileHelper FileHelper { get; }

        public ServerCommandHelper CommandHelper { get; }

        public ServerManagerMode ManagerMode { get; private set; }

        public ICommandSender CommandSender { get; private set; }

        public ServerLauncher? Launcher { get; private set; }

        public string ServerAddress { get; }

        public ushort ServerPort { get; }

        public ushort RconPort { get; }

        /// <summary>
        /// 当服务器写入日志时调用，并携带日志信息
        /// </summary>
        public event Action<MinecraftLog> OnWriteLog;

        /// <summary>
        /// 当服务器开始启动时调用
        /// </summary>
        public abstract event Action OnServerStarting;

        /// <summary>
        /// 当开始加载存档时调用，并携带存档名称
        /// </summary>
        public abstract event Action<string> OnPreparingLeveling;

        /// <summary>
        /// 当存档加载完成时调用，并携带加载耗时
        /// </summary>
        public abstract event Action<TimeSpan> OnPreparingLevelDone;

        /// <summary>
        /// 当服务器启动失败时调用，并携带错误信息
        /// </summary>
        public abstract event Action<string> OnServerStartFail;

        /// <summary>
        /// 当服务器崩溃时调用，并携带崩溃报告的UUID
        /// </summary>
        public abstract event Action<Guid> OnServerCrash;

        /// <summary>
        /// 当服务器开始终止时调用
        /// </summary>
        public abstract event Action OnServerStoping;

        /// <summary>
        /// 当服务器完成终止时调用
        /// </summary>
        public abstract event Action OnServerStopped;

        /// <summary>
        /// 当玩家登录服务器时调用，并携带玩家登录信息
        /// </summary>
        public abstract event Action<PlayerLoginInfo> OnPlayerJoined;

        /// <summary>
        /// 当玩家离开服务器时调用，并携带玩家登录信息与断开连接的原因
        /// </summary>
        public abstract event Action<PlayerLoginInfo?, string> OnPlayerLeft;

        /// <summary>
        /// 当玩家发送聊天消息时调用，并携带消息内容
        /// </summary>
        public abstract event Action<ChatMessage> OnPlayerSendChatMessage;

        /// <summary>
        /// 当RCON服务端开始运行时调用，并携带IP端口
        /// </summary>
        public abstract event Action<IPEndPoint> OnRconRunning;

        /// <summary>
        /// 当RCON服务端终止时调用
        /// </summary>
        public abstract event Action OnRconStopped;

        private void MinecraftServer_OnPlayerJoined(PlayerLoginInfo loginInfo)
        {
            _loginInfos.TryAdd(loginInfo.Name, loginInfo);
        }

        private void MinecraftServer_OnPlayerLeft(PlayerLoginInfo? loginInfo, string reason)
        {
            if (loginInfo is null)
                return;

            if (_loginInfos.ContainsKey(loginInfo.Name))
                _loginInfos.Remove(loginInfo.Name);
        }

        private void MinecraftServer_OnRconRunning(IPEndPoint obj)
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
            _logListener.WriteLog += (sender, e) => OnWriteLog.Invoke(e.MinecraftLog);
            CommandSender = Launcher;
            ManagerMode = ServerManagerMode.ManagedProcess;
            return Launcher;
        }

        public void ConnectExistingServer()
        {
            Encoding logEncoding = Encoding.UTF8;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                logEncoding = Encoding.GetEncoding("GBK");
            }
            LogFileListener logFileListener = new(PathManager.LatestLogFile, logEncoding);
            Task.Run(() => logFileListener.Start());
            _logListener = logFileListener;
            _logListener.WriteLog += (sender, e) => OnWriteLog.Invoke(e.MinecraftLog);
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
