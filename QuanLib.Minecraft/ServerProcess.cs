using log4net.Core;
using QuanLib.Core;
using QuanLib.Core.ExceptionHelper;
using QuanLib.Core.FileListeners;
using QuanLib.Minecraft.MinecraftLogs;
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
        public ServerProcess(string serverPath, ServerLaunchArguments launchArguments, Func<Type, LogImpl> logger) : base(logger)
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
            while (IsRuning)
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

        protected override void DisposeUnmanaged()
        {
            AutoRestart = false;
            if (!Process.HasExited)
                Process.Kill();
            Process.WaitForExit();
            Process.Dispose();
        }
    }
}
