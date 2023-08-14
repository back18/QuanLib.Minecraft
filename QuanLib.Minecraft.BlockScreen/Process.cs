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
                        MainThread.Start();
                    ProcessState = ProcessState.Running;
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

        public void StartProcess()
        {
            ProcessState = ProcessState.Starting;
        }

        public void StopProcess()
        {
            ProcessState = ProcessState.Stopped;
        }
    }
}
