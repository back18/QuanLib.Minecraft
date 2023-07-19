using QuanLib.Minecraft.BlockScreen.BuiltInApps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class Process : IMCOSComponent
    {
        public Process(ApplicationInfo appInfo, string[] args, Process? initiator = null)
        {
            ApplicationInfo = appInfo ?? throw new ArgumentNullException(nameof(appInfo));
            OnStart += () => { };
            OnStopped += () => { };
            Application = Application.CreateApplication(ApplicationInfo.TypeObject);
            Initiator = initiator;
            SubprocessCallbackQueue = new();
            MainThread = new(() =>
            {
                OnStart.Invoke();
                object? @return = Application.Main(args);
                OnStopped.Invoke();
                if (Initiator?.Initiator?.SubprocessCallbackQueue?.TryDequeue(out var callback) ?? false)
                    callback.Invoke(@return);
            })
            {
                Name = Application.AppID,
                IsBackground = true
            };
            IsPending = false;
        }

        public MCOS MCOS
        {
            get
            {
                if (_MCOS is null)
                    throw new InvalidOperationException();
                return _MCOS;
            }
            internal set => _MCOS = value;
        }
        private MCOS? _MCOS;

        public ApplicationInfo ApplicationInfo { get; }

        public Application Application { get; }

        public Process? Initiator { get; }

        public Queue<Action<object?>> SubprocessCallbackQueue { get; }

        public Thread MainThread { get; }

        public bool IsPending { get; internal set; }

        public ProcessState ProcessState
        {
            get
            {
                if (IsPending)
                    return ProcessState.Pending;
                else if (MainThread.ThreadState == ThreadState.Unstarted)
                    return ProcessState.Unstarted;
                else if (MainThread.IsAlive)
                    return ProcessState.Running;
                else
                    return ProcessState.Stopped;
            }
        }

        public event Action OnStart;

        public event Action OnStopped;

        public void Pending()
        {
            IsPending = true;
        }
    }
}
