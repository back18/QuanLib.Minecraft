#define DebugTimer

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.Files;
using QuanLib.BDF;
using SixLabors.ImageSharp;
using FFMediaToolkit;
using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.Frame;
using System.Collections.Concurrent;
using QuanLib.Minecraft.BlockScreen.Config;
using QuanLib.Minecraft.BlockScreen.Screens;
using QuanLib.Minecraft.BlockScreen.DirectoryManagers;

namespace QuanLib.Minecraft.BlockScreen
{
    public class MCOS : ISwitchable
    {
        static MCOS()
        {
            MainDirectory = new("MCBS");
        }

        public MCOS(MinecraftServer minecraftServer)
        {
            MinecraftServer = minecraftServer ?? throw new ArgumentNullException(nameof(minecraftServer));
            AccelerationEngine = new(ConfigManager.MinecraftConfig.ServerAddress, ConfigManager.MinecraftConfig.AccelerationEngineEventPort, ConfigManager.MinecraftConfig.AccelerationEngineDataPort);
            ApplicationManager = new();
            ScreenManager = new();
            ProcessManager = new();
            FormManager = new();

            EnableAccelerationEngine = ConfigManager.SystemConfig.EnableAccelerationEngine;
            FrameCount = 0;
            FrameMinTime = TimeSpan.FromMilliseconds(50);
            PreviousFrameTime = TimeSpan.Zero;
            NextFrameTime = PreviousFrameTime + FrameMinTime;
            SystemTimer = new();
            ServicesAppID = ConfigManager.SystemConfig.ServicesAppID;
            StartupChecklist = ConfigManager.SystemConfig.StartupChecklist;

            TaskList = new();
            TempTaskList = new();
            _stopwatch = new();

            _Instance = this;
        }

        public static MCOS Instance
        {
            get
            {
                if (_Instance is null)
                    throw new InvalidOperationException();
                return _Instance;
            }
        }
        private static MCOS? _Instance;

        public static McbsDirectory MainDirectory { get; }

        internal readonly ConcurrentQueue<Action> TaskList;

        internal readonly ConcurrentQueue<Action> TempTaskList;

        private readonly Stopwatch _stopwatch;

        private Task? _screen;

        private bool _runing;

        public bool Runing => _runing;

        public bool EnableAccelerationEngine { get; }

        public TimeSpan SystemRunningTime => _stopwatch.Elapsed;

        public TimeSpan FrameMinTime { get; set; }

        public TimeSpan PreviousFrameTime { get; private set; }

        public TimeSpan NextFrameTime { get; private set; }

        public int FrameCount { get; private set; }

        public SystemTimer SystemTimer { get; }

        public MinecraftServer MinecraftServer { get; }

        public AccelerationEngine AccelerationEngine { get; }

        public ApplicationManager ApplicationManager { get; }

        public ScreenManager ScreenManager { get; }

        public ProcessManager ProcessManager { get; }

        public FormManager FormManager { get; }

        public string ServicesAppID { get; }

        public IReadOnlyList<string> StartupChecklist { get; }

        public void Start()
        {
            _runing = true;

            if (EnableAccelerationEngine)
                AccelerationEngine.Start();
            ScreenManager.ScreenList.Add(new(new(440, 206, -90), 256, 144, Facing.Xm, Facing.Ym)).LoadScreen();

#if DebugTimer
            Console.CursorVisible = false;
#endif

            int lags = 0;
            _stopwatch.Start();
            while (_runing)
            {
                PreviousFrameTime = SystemRunningTime;
                NextFrameTime = PreviousFrameTime + FrameMinTime;
                FrameCount++;

                HandleScreenScheduling();
                HandleProcessScheduling();
                HandleFormScheduling();

                if (_screen?.IsCompleted ?? true)
                {
                    HandleScreenInput();
                }
                else
                {
                    TempTaskList.Enqueue(() => HandleScreenInput());
                    lags++;
                }

                HandleBeforeFrame();
                HandleUIRendering(out var frames);
                HandleScreenOutput(frames);
                HandleAfterFrame();
                HandleScreenBuild();
                HandleSystemInterrupt();

                SystemTimer.TotalTime.Add(SystemRunningTime - PreviousFrameTime);

#if DebugTimer
                string empty = new(' ', 200);
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < 14; i++)
                    Console.WriteLine(empty);
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(SystemTimer.ToString(Timer.Duration.Tick20));
                Console.WriteLine($"帧: {FrameCount}");
                Console.WriteLine($"滞后: {lags}");
#endif
            }

            _stopwatch.Stop();
        }

        public void Stop()
        {
            _runing = false;
        }

        private TimeSpan HandleScreenScheduling()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ScreenManager.ScreenScheduling();

            stopwatch.Stop();
            SystemTimer.ScreenScheduling.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleProcessScheduling()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ProcessManager.ProcessScheduling();

            stopwatch.Stop();
            SystemTimer.ProcessScheduling.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleFormScheduling()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            FormManager.FormScheduling();

            stopwatch.Stop();
            SystemTimer.FormScheduling.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleScreenInput()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ScreenManager.HandleAllScreenInput();

            stopwatch.Stop();
            SystemTimer.ScreenInput.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        public TimeSpan HandleAfterFrame()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ScreenManager.HandleAllAfterFrame();

            stopwatch.Stop();
            SystemTimer.HandleAfterFrame.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleUIRendering(out Dictionary<int, ArrayFrame> frames)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ScreenManager.HandleAllUIRendering(out frames);

            stopwatch.Stop();
            SystemTimer.UIRendering.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleScreenOutput(Dictionary<int, ArrayFrame> frames)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            _screen = ScreenManager.HandleAllScreenOutputAsync(frames);
            ScreenManager.WaitAllScreenPrevious();

            stopwatch.Stop();
            SystemTimer.ScreenOutput.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleBeforeFrame()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ScreenManager.HandleAllBeforeFrame();

            stopwatch.Stop();
            SystemTimer.HandleBeforeFrame.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleScreenBuild()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ScreenManager.ScreenBuilder.Handle();

            stopwatch.Stop();
            SystemTimer.ScreenBuild.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleSystemInterrupt()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            int time = (int)((NextFrameTime - SystemRunningTime).TotalMilliseconds - 10);
            if (time > 0)
                Thread.Sleep(time);
            while (SystemRunningTime < NextFrameTime)
                Thread.Yield();

            stopwatch.Stop();
            SystemTimer.SystemInterrupt.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        public ScreenContext? ScreenContextOf(IForm form)
        {
            if (form is null)
                throw new ArgumentNullException(nameof(form));

            foreach (var context in ScreenManager.ScreenList.Values)
                if (context.RootForm == form || context.RootForm.ContainsForm(form))
                    return context;

            return null;
        }

        public Process? ProcessOf(Application application)
        {
            if (application is null)
                throw new ArgumentNullException(nameof(application));

            foreach (var process in ProcessManager.ProcessList.Values)
                if (application == process.Application)
                    return process;

            return null;
        }

        public Process? ProcessOf(IForm form)
        {
            if (form is null)
                throw new ArgumentNullException(nameof(form));

            FormContext? context = FormContextOf(form);
            if (context is null)
                return null;

            return ProcessOf(context.Application);
        }

        public FormContext? FormContextOf(IForm form)
        {
            if (form is null)
                throw new ArgumentNullException(nameof(form));

            foreach (var context in FormManager.FormList)
                if (form == context.Form)
                    return context;

            return null;
        }

        public void AddTask(Action action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            TaskList.Enqueue(action);
        }

        public void AddTempTask(Action action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            TempTaskList.Enqueue(action);
        }

        internal Process RunServicesApp()
        {
            if (!ApplicationManager.ApplicationList[ServicesAppID].TypeObject.IsSubclassOf(typeof(ServicesApplication)))
                throw new InvalidOperationException("无效的ServicesAppID");

            Process process = ProcessManager.ProcessList.Add(ApplicationManager.ApplicationList[ServicesAppID]);
            process.StartProcess();
            return process;
        }

        internal void RunStartupChecklist(IRootForm rootForm)
        {
            foreach (var id in StartupChecklist)
                ProcessManager.ProcessList.Add(ApplicationManager.ApplicationList[id], rootForm).StartProcess();
        }
    }
}
