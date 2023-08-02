using QuanLib.Minecraft.Event;
using QuanLib.Minecraft.Files;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class ServerLauncher : ISwitchable, IStandardInputCommandSender, ILogListener
    {
        public ServerLauncher(string serverPath, ServerLaunchArguments launchArguments)
        {
            LaunchArguments = launchArguments ?? throw new ArgumentNullException(nameof(launchArguments));

            AutoRestart = false;
            StartCount = 0;
            MaxStartCount = 100;

            Process = new()
            {
                StartInfo = new(launchArguments.JavaPath, launchArguments.GetArguments())
                {
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    WorkingDirectory = serverPath
                }
            };

            WriteLog += OnWriteLog;
            ServerProcessStarted += OnServerProcessStarted;
            ServerProcessRestart += OnServerProcessRestart;
            ServerProcessExit += OnServerProcessExit;

            _runing = false;
        }

        private bool _runing;

        public bool Runing => _runing;

        public ServerLaunchArguments LaunchArguments { get; }

        public Process Process { get; }

        public bool AutoRestart { get; set; }

        public int StartCount { get; private set; }

        public int MaxStartCount
        {
            get => _MaxStartCount;
            set
            {
                if (value < 0)
                    _MaxStartCount = int.MaxValue;
                _MaxStartCount = value;
            }
        }
        private int _MaxStartCount;

        public event EventHandler<ILogListener, MinecraftLogEventArgs> WriteLog;

        public event EventHandler<ServerLauncher, EventArgs> ServerProcessStarted;

        public event EventHandler<ServerLauncher, EventArgs> ServerProcessRestart;

        public event EventHandler<ServerLauncher, EventArgs> ServerProcessExit;

        protected virtual void OnServerProcessStarted(ServerLauncher sender, EventArgs e)
        {
            Task.Run(() =>
            {
                while (!Process.StandardOutput.EndOfStream)
                {
                    string? line = Process.StandardOutput.ReadLine();
                    if (line is null)
                        continue;

                    if (line.StartsWith('['))
                    {
                        WriteLog.Invoke(this, new(new(line)));
                    }
                }
            });
        }

        protected virtual void OnServerProcessRestart(ServerLauncher sender, EventArgs e) { }

        protected virtual void OnServerProcessExit(ServerLauncher sender, EventArgs e) { }

        protected virtual void OnWriteLog(ILogListener sender, MinecraftLogEventArgs e) { }

        public void Start()
        {
            if (_runing)
                return;
            _runing = true;

            StartCount = 0;
            while (_runing)
            {
                StartCount++;

                Process.Start();
                ServerProcessStarted.Invoke(this, EventArgs.Empty);

                Process.WaitForExit();
                ServerProcessExit.Invoke(this, EventArgs.Empty);

                if (AutoRestart && StartCount < MaxStartCount)
                    ServerProcessRestart.Invoke(this, EventArgs.Empty);
                else
                    break;
            }
        }

        public void Stop()
        {
            _runing = false;
            Process.Kill();
        }

        public void SendCommand(string command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            Process.StandardInput.WriteLine(command);
        }

        public void SendAllCommand(IEnumerable<string> commands)
        {
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

            int length = 0;
            foreach (string command in commands)
                length += command.Length + 1;

            StringBuilder sb = new(length);
            foreach (string command in commands)
            {
                sb.Append(command);
                sb.Append('\n');
            }

            Process.StandardInput.Write(sb.ToString());
        }

        public async Task SendCommandAsync(string command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            await Process.StandardInput.WriteLineAsync(command);
        }

        public async Task SendAllCommandAsync(IEnumerable<string> commands)
        {
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

            int length = 0;
            foreach (string command in commands)
                length += command.Length + 1;

            StringBuilder sb = new(length);
            foreach (string command in commands)
            {
                sb.Append(command);
                sb.Append('\n');
            }

            await Process.StandardInput.WriteAsync(sb.ToString());
        }
    }
}
