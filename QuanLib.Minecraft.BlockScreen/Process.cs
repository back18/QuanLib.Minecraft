using log4net.Core;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Logging;
using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class Process
    {
        private static readonly LogImpl LOGGER = LogUtil.MainLogger;

        internal Process(ApplicationInfo appInfo, string[] args, IForm? initiator = null)
        {
            if (args is null)
                throw new ArgumentNullException(nameof(args));

            ApplicationInfo = appInfo ?? throw new ArgumentNullException(nameof(appInfo));
            Started += OnStarted;
            Stopped += OnStopped;
            Application = Application.CreateApplication(ApplicationInfo.TypeObject);
            Initiator = initiator;
            MainThread = new(() =>
            {
                ProcessState = ProcessState.Running;
                Started.Invoke(this, EventArgs.Empty);
                LOGGER.Info($"进程“{ToString()}”已启动");
                object? @return = Application.Main(args);
                ProcessState = ProcessState.Stopped;
                Stopped.Invoke(this, EventArgs.Empty);
                LOGGER.Info($"进程“{ToString()}”已退出");
            })
            {
                IsBackground = true
            };
            ID = -1;
        }

        public int ID { get; internal set; }

        public ApplicationInfo ApplicationInfo { get; }

        public Application Application { get; }

        public IForm? Initiator { get; }

        public Thread MainThread { get; }

        public ProcessState ProcessState { get; private set; }

        public event EventHandler<Process, EventArgs> Started;

        public event EventHandler<Process, EventArgs> Stopped;

        protected virtual void OnStarted(Process sender, EventArgs e) { }

        protected virtual void OnStopped(Process sender, EventArgs e)
        {
            foreach (var form in Application.GetForms())
            {
                form.CloseForm();
            }
        }

        public void Handle()
        {
            switch (ProcessState)
            {
                case ProcessState.Unstarted:
                    break;
                case ProcessState.Starting:
                    if (!MainThread.IsAlive)
                    {
                        MainThread.Name = $"{ApplicationInfo.ID}#{ID}";
                        MainThread.Start();
                    }
                    break;
                case ProcessState.Running:
                    break;
                case ProcessState.Stopped:
                    if (MainThread.IsAlive)
                    {
                        try
                        {
                            MainThread.Abort();
                        }
                        catch
                        {

                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public Process StartProcess()
        {
            ProcessState = ProcessState.Starting;
            return this;
        }

        public void StopProcess()
        {
            ProcessState = ProcessState.Stopped;
        }

        public override string ToString()
        {
            return $"State={ProcessState}, PID={ID}, AppID={ApplicationInfo.ID}";
        }
    }
}
