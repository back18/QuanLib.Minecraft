using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class ServerProcess : UnmanagedRunnable
    {
        public ServerProcess(string serverPath, ServerLaunchArguments launchArguments, string? extraArguments = null, ILoggerGetter? loggerGetter = null) : base(loggerGetter)
        {
            ArgumentException.ThrowIfNullOrEmpty(serverPath, nameof(serverPath));
            ArgumentNullException.ThrowIfNull(launchArguments, nameof(launchArguments));

            LaunchArguments = launchArguments;

            AutoRestart = false;
            StartCount = 0;
            MaxStartCount = 100;

            string arguments = launchArguments.GetArguments();
            if (!string.IsNullOrEmpty(extraArguments))
                arguments = string.Join(' ', extraArguments, launchArguments.GetArguments());

            Process = new()
            {
                StartInfo = new(launchArguments.JavaPath, arguments)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    WorkingDirectory = serverPath
                }
            };

            ServerProcessStarted += OnServerProcessStarted;
            ServerProcessRestart += OnServerProcessRestart;
            ServerProcessExited += OnServerProcessExited;
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
                if (value != -1 && value < 0)
                    ThrowHelper.ArgumentOutOfMin(0, value, nameof(value));
                _MaxStartCount = value;
            }
        }
        private int _MaxStartCount;

        public event EventHandler<ServerProcess, EventArgs> ServerProcessStarted;

        public event EventHandler<ServerProcess, EventArgs> ServerProcessRestart;

        public event EventHandler<ServerProcess, EventArgs> ServerProcessExited;

        protected virtual void OnServerProcessStarted(ServerProcess sender, EventArgs e) { }

        protected virtual void OnServerProcessRestart(ServerProcess sender, EventArgs e) { }

        protected virtual void OnServerProcessExited(ServerProcess sender, EventArgs e) { }

        protected override void Run()
        {
            StartCount = 0;
            while (IsRunning)
            {
                if (StartCount > 0)
                    ServerProcessRestart.Invoke(this, EventArgs.Empty);

                StartCount++;

                Process.Start();
                ServerProcessStarted.Invoke(this, EventArgs.Empty);

                Process.WaitForExit();
                ServerProcessExited.Invoke(this, EventArgs.Empty);

                if (!AutoRestart || (MaxStartCount != -1 && StartCount >= MaxStartCount))
                    break;
            }
        }

        public override void Stop()
        {
            try
            {
                Process.StandardInput.WriteLine("stop");
            }
            catch (InvalidOperationException)
            {

            }

            base.Stop();
        }

        protected override void DisposeUnmanaged()
        {
            try
            {
                DateTime startTime = Process.StartTime;     //成功获取到启动时间代表进程已启动，非则抛出 InvalidOperationException
            }
            catch (InvalidOperationException)
            {
                Process.Dispose();                          //如果进程从未启动，直接 Dispose 即可
                return;
            }

            AutoRestart = false;
            if (!Process.HasExited)
                Process.Kill();
            Process.WaitForExit();
            Process.Dispose();
        }
    }
}
