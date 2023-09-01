#define TryCatch

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using QuanLib.BDF;
using SixLabors.ImageSharp;
using FFMediaToolkit;
using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.Frame;
using System.Collections.Concurrent;
using QuanLib.Minecraft.BlockScreen.Config;
using QuanLib.Minecraft.BlockScreen.Screens;
using QuanLib.Minecraft.BlockScreen.DirectoryManagers;
using log4net.Core;
using QuanLib.Minecraft.BlockScreen.Logging;
using System.Runtime.CompilerServices;
using QuanLib.Core;

namespace QuanLib.Minecraft.BlockScreen
{
    public class MCOS : ISwitchable, IDisposable
    {
        private static LogImpl LOGGER => LogUtil.MainLogger;

        static MCOS()
        {
            _lock = new();
            IsLoaded = false;
            MainDirectory = new(Path.GetFullPath("MCBS"));
        }

        private MCOS(MinecraftServer minecraftServer)
        {
            MinecraftServer = minecraftServer ?? throw new ArgumentNullException(nameof(minecraftServer));
            AccelerationEngine = new(ConfigManager.MinecraftConfig.ServerAddress, ConfigManager.MinecraftConfig.AccelerationEngineEventPort, ConfigManager.MinecraftConfig.AccelerationEngineDataPort);
            ApplicationManager = new();
            ScreenManager = new();
            ProcessManager = new();
            FormManager = new();
            InteractionManager = new();

            EnableAccelerationEngine = ConfigManager.SystemConfig.EnableAccelerationEngine;
            FrameCount = 0;
            LagFrameCount = 0;
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

        private static readonly object _lock;

        public static bool IsLoaded { get; private set; }

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

        private bool _runing;

        public bool Runing => _runing;

        public bool EnableAccelerationEngine { get; private set; }

        public TimeSpan SystemRunningTime => _stopwatch.Elapsed;

        public TimeSpan FrameMinTime { get; set; }

        public TimeSpan PreviousFrameTime { get; private set; }

        public TimeSpan NextFrameTime { get; private set; }

        public int FrameCount { get; private set; }

        public int LagFrameCount { get; private set; }

        public SystemTimer SystemTimer { get; }

        public MinecraftServer MinecraftServer { get; }

        public AccelerationEngine AccelerationEngine { get; }

        public ApplicationManager ApplicationManager { get; }

        public ScreenManager ScreenManager { get; }

        public ProcessManager ProcessManager { get; }

        public FormManager FormManager { get; }

        public InteractionManager InteractionManager { get; }

        public string ServicesAppID { get; }

        public IReadOnlyList<string> StartupChecklist { get; }

        public static MCOS Load(MinecraftServer server)
        {
            if (server is null)
                throw new ArgumentNullException(nameof(server));

            lock (_lock)
            {
                _Instance ??= new(server);
                IsLoaded = true;
                return _Instance;
            }
        }

        private void Initialize()
        {
            LOGGER.Info("开始初始化");

            LOGGER.Info("正在等待Minecraft服务器启动...");
            MinecraftServer.WaitForConnected();
            LOGGER.Info("成功连接到Minecraft服务器");

#if TryCatch
            try
            {
#endif
                if (EnableAccelerationEngine)
                {
                    LOGGER.Info($"加速引擎已启用，正在连接加速引擎服务器\n服务器地址:{AccelerationEngine.ServerAddress}\n事件端口:{AccelerationEngine.EventPort}\n数据端口:{AccelerationEngine.DataPort}");
                    AccelerationEngine.Start();
                    LOGGER.Info("已连接到加速引擎服务器");
                }
#if TryCatch
            }
            catch (Exception ex)
            {
                EnableAccelerationEngine = false;
                LOGGER.Error("无法连接到加速引擎服务器，加速引擎已禁用", ex);
            }
#endif

            ScreenManager.Initialize();
            InteractionManager.Initialize();
            MinecraftServer.CommandHelper.SendCommand($"scoreboard objectives add {ConfigManager.ScreenConfig.RightClickObjective} minecraft.used:minecraft.snowball");

            LOGGER.Info("初始化完成");
        }

        public void Start()
        {
            if (_runing)
                return;

            _runing = true;
            LOGGER.Info("系统已开始运行");
            Initialize();
            _stopwatch.Start();

            run:

#if TryCatch
            try
            {
#endif
                while (_runing)
                {
                    PreviousFrameTime = SystemRunningTime;
                    NextFrameTime = PreviousFrameTime + FrameMinTime;
                    FrameCount++;

                    HandleScreenScheduling();
                    HandleProcessScheduling();
                    HandleFormScheduling();
                    HandleInteractionScheduling();

                    if (ScreenManager.IsCompletedOutput)
                    {
                        HandleScreenInput();
                        HandleScreenBuild();
                    }
                    else
                    {
                        AddTempTask(() => HandleScreenInput());
                        AddTempTask(() => HandleScreenBuild());
                        LagFrameCount++;
                    }

                    HandleBeforeFrame();
                    HandleUIRendering(out var frames);
                    HandleScreenOutput(frames);
                    HandleAfterFrame();
                    HandleSystemInterrupt();

                    SystemTimer.TotalTime.Add(SystemRunningTime - PreviousFrameTime);
                }
#if TryCatch
            }
            catch (Exception ex)
            {
                bool connect = MinecraftServer.PingServer(out _) && MinecraftServer.PingRcon(out _);

                if (!connect)
                {
                    LOGGER.Fatal("系统运行时遇到意外错误，并且无法继续连接到Minecraft服务器，系统即将终止运行", ex);
                }
                else if (ConfigManager.SystemConfig.CrashAutoRestart)
                {
                    foreach (var context in ScreenManager.Items.Values)
                    {
                        context.RestartScreen();
                    }
                    LOGGER.Error("系统运行时遇到意外错误，已启用自动重启，系统即将在3秒后重启", ex);
                    for (int i = 3; i >= 1; i--)
                    {
                        LOGGER.Info($"将在{i}秒后自动重启...");
                        Thread.Sleep(1000);
                    }
                    ScreenManager.ClearOutputTask();
                    LOGGER.Info("开始重启...");
                    goto run;
                }
                else
                {
                    LOGGER.Fatal("系统运行时遇到意外错误，未启用自动重启，系统即将终止运行", ex);
                }
            }
#endif

            _stopwatch.Stop();
            Dispose();
            _runing = false;
            LOGGER.Info("系统已终止运行");
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

        private TimeSpan HandleInteractionScheduling()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            InteractionManager.InteractionScheduling();

            stopwatch.Stop();
            SystemTimer.InteractionScheduling.Add(stopwatch.Elapsed);
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

            _ = ScreenManager.HandleAllScreenOutputAsync(frames);
            ScreenManager.WaitAllScreenPreviousOutputTask();

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

            foreach (var context in ScreenManager.Items.Values)
                if (context.RootForm == form || context.RootForm.ContainsForm(form))
                    return context;

            return null;
        }

        public Process? ProcessOf(Application application)
        {
            if (application is null)
                throw new ArgumentNullException(nameof(application));

            foreach (var process in ProcessManager.Items.Values)
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

            foreach (var context in FormManager.Items.Values.ToArray())
                if (form == context.Form)
                    return context;

            return null;
        }

        public Process RunApplication(ApplicationInfo appInfo, IForm? initiator = null)
        {
            if (appInfo is null)
                throw new ArgumentNullException(nameof(appInfo));

            return ProcessManager.Items.Add(appInfo, initiator).StartProcess();
        }

        public Process RunApplication(ApplicationInfo appInfo, string[] args, IForm? initiator = null)
        {
            if (appInfo is null)
                throw new ArgumentNullException(nameof(appInfo));
            if (args is null)
                throw new ArgumentNullException(nameof(args));

            return ProcessManager.Items.Add(appInfo, args, initiator).StartProcess();
        }

        public Process RunApplication(string appID, string[] args, IForm? initiator = null)
        {
            if (string.IsNullOrEmpty(appID))
                throw new ArgumentException($"“{nameof(appID)}”不能为 null 或空。", nameof(appID));

            return ProcessManager.Items.Add(ApplicationManager.Items[appID], args, initiator).StartProcess();
        }

        public Process RunApplication(string appID, IForm? initiator = null)
        {
            if (string.IsNullOrEmpty(appID))
                throw new ArgumentException($"“{nameof(appID)}”不能为 null 或空。", nameof(appID));

            return ProcessManager.Items.Add(ApplicationManager.Items[appID], initiator).StartProcess();
        }

        public ScreenContext LoadScreen(Screen screen)
        {
            if (screen is null)
                throw new ArgumentNullException(nameof(screen));

            return ScreenManager.Items.Add(screen).LoadScreen();
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
            if (!ApplicationManager.Items[ServicesAppID].TypeObject.IsSubclassOf(typeof(ServicesApplication)))
                throw new InvalidOperationException("无效的ServicesAppID");

            return RunApplication(ServicesAppID);
        }

        internal void RunStartupChecklist(IRootForm rootForm)
        {
            foreach (var id in StartupChecklist)
                RunApplication(ApplicationManager.Items[id], rootForm);
        }

        public void Dispose()
        {
            LOGGER.Info("开始释放非托管资源");

            bool connect = MinecraftServer.PingServer(out _) && MinecraftServer.PingRcon(out _);
            if (!connect)
            {
                LOGGER.Warn("无法继续连接到Minecraft服务器，因此无法释放托管在Minecraft中的资源");
                return;
            }

            foreach (var context in ScreenManager.Items.Values)
            {
                context.Screen.Fill();
                context.Screen.UnloadScreenChunks();
            }

            foreach (var interaction in InteractionManager.Items.Values)
            {
                interaction.Dispose();
            }

            GC.SuppressFinalize(this);

            LOGGER.Info("非托管资源释放完成");
        }
    }
}
