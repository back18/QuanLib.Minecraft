using QuanLib.Core;
using QuanLib.Core.Events;
using QuanLib.Minecraft.Logging.LogEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Logging
{
    public class LogAnalyzer
    {
        public LogAnalyzer(ILogListener listener)
        {
            ArgumentNullException.ThrowIfNull(listener, nameof(listener));

            _listener = listener;
            _publishers = [];

            Enable = false;

            Starting += OnStarting;
            Stopping += OnStopping;
            RconRunning += OnRconRunning;
            RconStopped += OnRconStopped;
            PlayerLogin += OnPlayerLogin;
            PlayerLeft += OnPlayerLeft;
            ChatMessage += OnChatMessage;
            Crashed += OnCrashed;

            _publishers.Add(new StartingEventPublisher((sender, e) => Starting.Invoke(this, e), this));
            _publishers.Add(new StoppingEventPublisher((sender, e) => Stopping.Invoke(this, e), this));
            _publishers.Add(new RconRunningEventPublisher((sender, e) =>
            {
                if (e is EventArgs<IPEndPoint> eventArgs)
                    RconRunning.Invoke(this, eventArgs);
            }, this));
            _publishers.Add(new RconStoppedEventPublisher((sender, e) => RconStopped.Invoke(this, e), this));
            _publishers.Add(new PlayerLoginEventPublisher((sender, e) =>
            {
                if (e is EventArgs<PlayerLoginInfo> eventArgs)
                    PlayerLogin.Invoke(this, eventArgs);
            }, this));
            _publishers.Add(new PlayerLeftEventPublisher((sender, e) =>
            {
                if (e is EventArgs<PlayerLeftInfo> eventArgs)
                    PlayerLeft.Invoke(this, eventArgs);
            }, this));
            _publishers.Add(new ChatMessageEventPublisher((sender, e) =>
            {
                if (e is EventArgs<ChatMessageInfo> eventArgs)
                    ChatMessage.Invoke(this, eventArgs);
            }, this));
            _publishers.Add(new CrashedEventPublisher((sender, e) => Crashed.Invoke(this, e), this));

            _listener.WriteLog += LogListener_WriteLog;
        }

        private readonly ILogListener _listener;

        private readonly List<ILogEventPublisher> _publishers;

        public bool Enable { get; set; }

        public event EventHandler<LogAnalyzer, EventArgs> Starting;

        public event EventHandler<LogAnalyzer, EventArgs> Stopping;

        public event EventHandler<LogAnalyzer, EventArgs<IPEndPoint>> RconRunning;

        public event EventHandler<LogAnalyzer, EventArgs> RconStopped;

        public event EventHandler<LogAnalyzer, EventArgs<PlayerLoginInfo>> PlayerLogin;

        public event EventHandler<LogAnalyzer, EventArgs<PlayerLeftInfo>> PlayerLeft;

        public event EventHandler<LogAnalyzer, EventArgs<ChatMessageInfo>> ChatMessage;

        public event EventHandler<LogAnalyzer, EventArgs> Crashed;

        protected virtual void OnStarting(LogAnalyzer sender, EventArgs e) { }

        protected virtual void OnStopping(LogAnalyzer sender, EventArgs e) { }

        protected virtual void OnRconRunning(LogAnalyzer sender, EventArgs<IPEndPoint> e) { }

        protected virtual void OnRconStopped(LogAnalyzer sender, EventArgs e) { }

        protected virtual void OnPlayerLogin(LogAnalyzer sender, EventArgs<PlayerLoginInfo> e) { }

        protected virtual void OnPlayerLeft(LogAnalyzer sender, EventArgs<PlayerLeftInfo> e) { }

        protected virtual void OnChatMessage(LogAnalyzer sender, EventArgs<ChatMessageInfo> e) { }

        protected virtual void OnCrashed(LogAnalyzer sender, EventArgs e) { }

        public void Publish(ILogEventPublisher publisher)
        {
            ArgumentNullException.ThrowIfNull(publisher, nameof(publisher));

            _publishers.Add(publisher);
        }

        private void LogListener_WriteLog(ILogListener sender, ValueEventArgs<MinecraftLog> e)
        {
            if (!Enable)
                return;

            MinecraftLog log = e.Argument;
            for (int i = 0; i < _publishers.Count; i++)
            {
                ILogEventPublisher publisher = _publishers[i];

                if (!publisher.Match(log))
                    continue;

                if (!publisher.TryParse(log.Message, out var eventArgs))
                    continue;

                publisher.TriggerEvent(eventArgs);
                break;
            }
        }
    }
}
