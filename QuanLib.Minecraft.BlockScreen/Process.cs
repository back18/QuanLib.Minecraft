using QuanLib.Minecraft.BlockScreen.Event;
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
        public Process(ApplicationInfo appInfo, string[] args, IForm? initiator = null)
        {
            if (args is null)
                throw new ArgumentNullException(nameof(args));

            ApplicationInfo = appInfo ?? throw new ArgumentNullException(nameof(appInfo));
            Started += OnStarted;
            Stopped += OnStopped;
            Application = Application.CreateApplication(ApplicationInfo.TypeObject);
            Initiator = initiator;
            SubprocessCallbackQueue = new();
            MainThread = new(() =>
            {
                Started.Invoke(this, EventArgs.Empty);
                object? @return = Application.Main(args);
                Stopped.Invoke(this, EventArgs.Empty);
            })
            {
                Name = ApplicationInfo.Name,
                IsBackground = true
            };
            IsPending = false;
            ID = -1;
        }

        public int ID { get; internal set; }

        public ApplicationInfo ApplicationInfo { get; }

        public Application Application { get; }

        public IForm? Initiator { get; }

        public Queue<Action<object?>> SubprocessCallbackQueue { get; }

        public Thread MainThread { get; }

        public bool IsPending { get; internal set; }

        public ProcessState ProcessState
        {
            get
            {
                if (MainThread.ThreadState == ThreadState.Unstarted)
                    return ProcessState.Unstarted;
                else if (MainThread.IsAlive)
                    return ProcessState.Running;
                else
                    return ProcessState.Stopped;
            }
        }

        public event EventHandler<Process, EventArgs> Started;

        public event EventHandler<Process, EventArgs> Stopped;

        protected virtual void OnStarted(Process sender, EventArgs e) { }

        protected virtual void OnStopped(Process sender, EventArgs e) { }
    }
}
