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

namespace QuanLib.Minecraft.BlockScreen
{
    public class MCOS : ISwitchable
    {
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
            Timer = new();
            ServicesAppID = ConfigManager.SystemConfig.ServicesAppID;
            StartupChecklist = ConfigManager.SystemConfig.StartupChecklist;

            _callbacks = new();
            _stopwatch = new();

            _mcos = this;
        }

        public static MCOS Instance
        {
            get
            {
                if (_mcos is null)
                    throw new InvalidOperationException();
                return _mcos;
            }
        }

        private static MCOS? _mcos;

        internal readonly ConcurrentQueue<Action> _callbacks;

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

        public SystemTimer Timer { get; }

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
            ScreenManager.ScreenList.Add(new(new(440, 206, -90), Facing.Xm, Facing.Ym, 256, 144));

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

                foreach (var context in ScreenManager.ScreenList.Values.ToArray())
                    if (context.ScreenState == ScreenState.Closed)
                        ScreenManager.ScreenList.Remove(context.ID);

                HandleProcessScheduling();

                ScreenManager.ScreenConstructor.Handle();
                if (_screen?.IsCompleted ?? true)
                {
                    HandleScreenInput();
                }
                else
                {
                    _callbacks.Enqueue(() => HandleScreenInput());
                    lags++;
                }

                HandleBeforeFrame();
                HandleUIRendering(out var frames);
                HandleScreenOutput(frames);
                HandleAfterFrame();
                HandleSystemInterrupt();

                Timer.TotalTime.Add(SystemRunningTime - PreviousFrameTime);

#if DebugTimer
                string empty = new(' ', 200);
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < 10; i++)
                    Console.WriteLine(empty);
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(Timer.ToString(BlockScreen.Timer.Duration.Tick20));
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

        private TimeSpan HandleProcessScheduling()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            //TODO

            stopwatch.Stop();
            Timer.ProcessScheduling.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleScreenInput()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ScreenManager.HandleAllScreenInput();

            stopwatch.Stop();
            Timer.ScreenInput.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleBeforeFrame()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ScreenManager.HandleAllBeforeFrame();

            stopwatch.Stop();
            Timer.HandleBeforeFrame.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        public TimeSpan HandleAfterFrame()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ScreenManager.HandleAllAfterFrame();

            stopwatch.Stop();
            Timer.HandleAfterFrame.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleUIRendering(out Dictionary<int, ArrayFrame> frames)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ScreenManager.HandleAllUIRendering(out frames);

            stopwatch.Stop();
            Timer.UIRendering.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleScreenOutput(Dictionary<int, ArrayFrame> frames)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            _screen = ScreenManager.HandleAllScreenOutputAsync(frames);
            ScreenManager.WaitAllScreenPrevious();

            stopwatch.Stop();
            Timer.ScreenOutput.Add(stopwatch.Elapsed);
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
            Timer.SystemInterrupt.Add(stopwatch.Elapsed);
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

        public ScreenContext CreateScreenContext(Screen screen)
        {
            if (screen is null)
                throw new ArgumentNullException(nameof(screen));

            Process process = RunServicesApp();
            IRootForm rootForm = ((ServicesApplication)process.Application).RootForm;
            rootForm.HandleAllInitialize();
            rootForm.ClientSize = screen.Size;
            RunStartupChecklist(rootForm);
            return new(screen, rootForm);
        }

        private Process RunServicesApp()
        {
            if (!ApplicationManager.ApplicationList[ServicesAppID].TypeObject.IsSubclassOf(typeof(ServicesApplication)))
                throw new InvalidOperationException("无效的ServicesAppID");

            return ProcessManager.ProcessList.Add(ApplicationManager.ApplicationList[ServicesAppID]);
        }

        private void RunStartupChecklist(IForm? initiator = null)
        {
            foreach (var id in StartupChecklist)
                ProcessManager.ProcessList.Add(ApplicationManager.ApplicationList[id], initiator);
        }
    }
}
