using QuanLib.Minecraft.Files;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class ServeLauncher : IStandardInputCommandSender, ILogListener
    {
        public ServeLauncher(string serverPath, ServerLaunchArguments launchArguments)
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

            OnWriteLog += (obj) => { };
            OnServerProcessStart += ServeLauncher_OnServerProcessStart;
            OnServerProcessRestart += () => { };
            OnServerProcessExit += () => { };

            _temp = new();
        }

        private readonly StringBuilder _temp;

        public event Action<MinecraftLog> OnWriteLog;

        public event Action OnServerProcessStart;

        public event Action OnServerProcessRestart;

        public event Action OnServerProcessExit;

        private void ServeLauncher_OnServerProcessStart()
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
                        if (_temp.Length > 0)
                        {
                            _temp.Remove(_temp.Length - 1, 1);
                            if (MinecraftLog.TryParse(_temp.ToString(), out var result1))
                                OnWriteLog.Invoke(result1);
                            _temp.Clear();
                        }

                        if (MinecraftLog.TryParse(line, out var result2))
                        {
                            if (result2.Level != Level.ERROR)
                            {
                                OnWriteLog.Invoke(result2);
                            }
                            else
                            {
                                _temp.Append(line);
                                _temp.Append('\n');
                            }
                        }
                    }
                    else if (_temp.Length > 0 && _temp[0] == '[')
                    {
                        _temp.Append(line);
                        _temp.Append('\n');
                    }
                    else
                    {

                    }
                }
            });
        }

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

        public void Start()
        {
            StartCount = 0;
            while (true)
            {
                StartCount++;

                Process.Start();
                OnServerProcessStart.Invoke();

                Process.WaitForExit();
                OnServerProcessExit.Invoke();

                if (AutoRestart && StartCount < MaxStartCount)
                    OnServerProcessRestart.Invoke();
                else
                    break;
            }
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
