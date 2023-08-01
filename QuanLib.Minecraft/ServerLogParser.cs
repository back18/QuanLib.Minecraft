using QuanLib.Event;
using QuanLib.Minecraft.Event;
using QuanLib.Minecraft.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class ServerLogParser
    {
        public ServerLogParser(ILogListener listener)
        {
            _listener = listener ?? throw new ArgumentNullException(nameof(listener));
            _listener.WriteLog += LogListener_WriteLog;

            Starting += OnStarting;
            Started += OnStarted;
            Stopping += OnStopping;
            Stopped += OnStarted;
            FailToStart += OnFailToStart;
            Crashed += OnCrashed;
            RconRunning += OnRconRunning;
            RconStopped += OnRconStopped;
            PreparingLevel += OnPreparingLevel;
            PlayerJoined += OnPlayerJoined;
            PlayerLeft += OnPlayerLeft;
            PlayerSendChatMessage += OnPlayerSendChatMessage;
        }

        private readonly ILogListener _listener;

        public event EventHandler<ServerLogParser, TextEventArgs> Starting;

        public event EventHandler<ServerLogParser, EventArgs> Started;

        public event EventHandler<ServerLogParser, EventArgs> Stopping;

        public event EventHandler<ServerLogParser, EventArgs> Stopped;

        public event EventHandler<ServerLogParser, TextEventArgs> FailToStart;

        public event EventHandler<ServerLogParser, GuidEventArgs> Crashed;

        public event EventHandler<ServerLogParser, IPEndPointEventArgs> RconRunning;

        public event EventHandler<ServerLogParser, EventArgs> RconStopped;

        public event EventHandler<ServerLogParser, TextEventArgs> PreparingLevel;

        public event EventHandler<ServerLogParser, PlayerLoginInfoEventArgs> PlayerJoined;

        public event EventHandler<ServerLogParser, PlayerLeftInfoEventArgs> PlayerLeft;

        public event EventHandler<ServerLogParser, ChatMessageEventArgs> PlayerSendChatMessage;

        protected virtual void OnStarting(ServerLogParser sender, TextEventArgs e) { }

        protected virtual void OnStarted(ServerLogParser sender, EventArgs e) { }

        protected virtual void OnStopping(ServerLogParser sender, EventArgs e) { }

        protected virtual void OnStopped(ServerLogParser sender, EventArgs e) { }

        protected virtual void OnFailToStart(ServerLogParser sender, TextEventArgs e) { }

        protected virtual void OnCrashed(ServerLogParser sender, GuidEventArgs e) { }

        protected virtual void OnRconRunning(ServerLogParser sender, IPEndPointEventArgs e) { }

        protected virtual void OnRconStopped(ServerLogParser sender, EventArgs e) { }

        protected virtual void OnPreparingLevel(ServerLogParser sender, TextEventArgs e) { }

        protected virtual void OnPlayerJoined(ServerLogParser sender, PlayerLoginInfoEventArgs e) { }

        protected virtual void OnPlayerLeft(ServerLogParser sender, PlayerLeftInfoEventArgs e) { }

        protected virtual void OnPlayerSendChatMessage(ServerLogParser sender, ChatMessageEventArgs e) { }

        private void LogListener_WriteLog(ILogListener sender, MinecraftLogEventArgs e)
        {
            string message = e.MinecraftLog.Message;

            if (string.IsNullOrEmpty(message))
                return;
            else if (message.StartsWith("Starting minecraft server"))
            {
                Starting.Invoke(this, new(message.Split(' ')[^1]));
            }
            else if (message.EndsWith("For help, type \"help\""))
            {
                Started.Invoke(this, EventArgs.Empty);
            }
            else if (message.StartsWith("Stopping server"))
            {
                Stopping.Invoke(this, EventArgs.Empty);
            }
            else if (message.EndsWith("All dimensions are saved"))
            {
                Stopped.Invoke(this, EventArgs.Empty);
            }
            else if (message.StartsWith("Failed to start the minecraft server"))
            {
                FailToStart.Invoke(this, new(message));
            }
            else if (message.StartsWith("Preparing crash report with UUID"))
            {
                _ = Guid.TryParse(message.Split(' ')[^1], out var uuid);
                Crashed.Invoke(this, new(uuid));
            }
            else if (message.StartsWith("RCON running"))
            {
                if (!IPEndPoint.TryParse(message.Split(' ')[^1], out var ipPort))
                    ipPort = IPEndPoint.Parse("0.0.0.0:25575");
                RconRunning.Invoke(this, new(ipPort));
            }
            else if (message.StartsWith("Thread RCON Listener stopped"))
            {
                RconStopped.Invoke(this, EventArgs.Empty);
            }
            else if (message.StartsWith("Preparing level"))
            {
                Match match = Regex.Match(message, "\"([^\"]*)\"");
                string name;
                if (match.Success)
                    name = match.Groups[1].Value;
                else
                    name = message.Split(" ")[^1];
                PreparingLevel.Invoke(this, new(name));
            }
            else if (message.Contains("logged in with entity"))
            {
                if (!PlayerLoginInfo.TryParse(message, out var loginInfo))
                    loginInfo = new(string.Empty, IPAddress.Any, 0, 0, new(0, 0, 0));
                PlayerJoined.Invoke(this, new(loginInfo));
            }
            else if (message.Contains("lost connection"))
            {
                Match match2 = Regex.Match(message, @"^(?<name>\w+) lost connection: (?<reason>\w+)$");
                PlayerLeftInfo leftInfo;
                if (match2.Success)
                    leftInfo = new(match2.Groups["name"].Value, match2.Groups["reason"].Value);
                else
                    leftInfo = new(string.Empty, string.Empty);
                PlayerLeft.Invoke(this, new(leftInfo));
            }
            else if (message.Contains('<') && message.Contains('>'))
            {
                Match match3 = Regex.Match(message, @"<(.*?)>\s*(.*)");
                ChatMessage chatMessage;
                if (match3.Success)
                    chatMessage = new(match3.Groups[1].Value.Trim(), match3.Groups[2].Value.Trim());
                else
                    chatMessage = new(string.Empty, string.Empty);
                PlayerSendChatMessage.Invoke(this, new(chatMessage));
            }
        }
    }
}
