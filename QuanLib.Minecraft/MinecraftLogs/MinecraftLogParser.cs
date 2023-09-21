using QuanLib.Core;
using QuanLib.Core.Event;
using QuanLib.Minecraft.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.MinecraftLogs
{
    public class MinecraftLogParser : IBindable
    {
        public MinecraftLogParser(ILogListener listener)
        {
            LogListener = listener ?? throw new ArgumentNullException(nameof(listener));
            IsBound = false;

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

        public ILogListener LogListener { get; }

        public bool IsBound { get; protected set; }

        public event EventHandler<MinecraftLogParser, TextEventArgs> Starting;

        public event EventHandler<MinecraftLogParser, EventArgs> Started;

        public event EventHandler<MinecraftLogParser, EventArgs> Stopping;

        public event EventHandler<MinecraftLogParser, EventArgs> Stopped;

        public event EventHandler<MinecraftLogParser, TextEventArgs> FailToStart;

        public event EventHandler<MinecraftLogParser, GuidEventArgs> Crashed;

        public event EventHandler<MinecraftLogParser, IPEndPointEventArgs> RconRunning;

        public event EventHandler<MinecraftLogParser, EventArgs> RconStopped;

        public event EventHandler<MinecraftLogParser, TextEventArgs> PreparingLevel;

        public event EventHandler<MinecraftLogParser, PlayerLoginInfoEventArgs> PlayerJoined;

        public event EventHandler<MinecraftLogParser, PlayerLeftInfoEventArgs> PlayerLeft;

        public event EventHandler<MinecraftLogParser, ChatMessageEventArgs> PlayerSendChatMessage;

        protected virtual void OnStarting(MinecraftLogParser sender, TextEventArgs e) { }

        protected virtual void OnStarted(MinecraftLogParser sender, EventArgs e) { }

        protected virtual void OnStopping(MinecraftLogParser sender, EventArgs e) { }

        protected virtual void OnStopped(MinecraftLogParser sender, EventArgs e) { }

        protected virtual void OnFailToStart(MinecraftLogParser sender, TextEventArgs e) { }

        protected virtual void OnCrashed(MinecraftLogParser sender, GuidEventArgs e) { }

        protected virtual void OnRconRunning(MinecraftLogParser sender, IPEndPointEventArgs e) { }

        protected virtual void OnRconStopped(MinecraftLogParser sender, EventArgs e) { }

        protected virtual void OnPreparingLevel(MinecraftLogParser sender, TextEventArgs e) { }

        protected virtual void OnPlayerJoined(MinecraftLogParser sender, PlayerLoginInfoEventArgs e) { }

        protected virtual void OnPlayerLeft(MinecraftLogParser sender, PlayerLeftInfoEventArgs e) { }

        protected virtual void OnPlayerSendChatMessage(MinecraftLogParser sender, ChatMessageEventArgs e) { }

        protected virtual void LogListener_WriteLog(ILogListener sender, MinecraftLogEventArgs e)
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

        public void Bind()
        {
            LogListener.WriteLog += LogListener_WriteLog;
        }

        public void Unbind()
        {
            LogListener.WriteLog -= LogListener_WriteLog;
        }
    }
}
