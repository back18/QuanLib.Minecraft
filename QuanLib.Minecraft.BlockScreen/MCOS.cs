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
        static MCOS()
        {
            Task<BlockTextureCollection> task2 = Task.Run(() => BlockTextureCollection.Load(Path.Combine(PathManager.MinecraftResources_Dir, "assets", "minecraft")));
            Task<BdfFont> task3 = Task.Run(() => BdfFont.Load(Path.Combine(PathManager.SystemResources_Fonts_Dir, "DefaultFont.bdf")));
            Task<Dictionary<CursorType, Cursor>> task4 = Task.Run(() =>
            {
                Dictionary<CursorType, Cursor> result = new();
                string[] files = Directory.GetFiles(PathManager.SystemResources_Textures_Cursor_Dir);
                foreach (string file in files)
                {
                    Cursor cursor = new(JsonConvert.DeserializeObject<Cursor.Json>(File.ReadAllText(file)) ?? throw new FormatException());
                    result.Add(cursor.CursorType, cursor);
                }
                return result;
            });
            Task task5 = Task.Run(() =>
            {
                FFmpegLoader.FFmpegPath = PathManager.FFmpeg_Dir;
                FFmpegLoader.LoadFFmpeg();
            });

            BlockTextureCollection = task2.Result;
            DefaultFont = task3.Result;
            _cursors = task4.Result;
            task5.Wait();

            _fonts = new();
            RegisterFont("DefaultFont", DefaultFont);
        }

        public MCOS(
            MinecraftServer minecraftServer,
            AccelerationEngine accelerationEngine)
        {
            MinecraftServer = minecraftServer ?? throw new ArgumentNullException(nameof(minecraftServer));
            AccelerationEngine = accelerationEngine ?? throw new ArgumentNullException(nameof(accelerationEngine));
            ScreenManager = new();
            ProcessManager = new();
            FormManager = new();

            EnableAccelerationEngine = true;
            FrameCount = 0;
            FrameMinTime = TimeSpan.FromMilliseconds(50);
            PreviousFrameTime = TimeSpan.Zero;
            NextFrameTime = PreviousFrameTime + FrameMinTime;
            Timer = new();
            Operator = string.Empty;
            CursorType = CursorType.Default;
            ServicesAppID = ConfigManager.SystemConfig.ServicesAppID;
            StartupChecklist = ConfigManager.SystemConfig.StartupChecklist;

            _apps = new();
            _callbacks = new();
            _stopwatch = new();

            _mcos = this;
        }

        private static MCOS? _mcos;

        private static readonly Dictionary<string, BdfFont> _fonts;

        public static readonly Dictionary<CursorType, Cursor> _cursors;

        private readonly Dictionary<string, ApplicationInfo> _apps;

        internal readonly ConcurrentQueue<Action> _callbacks;

        private readonly Stopwatch _stopwatch;

        private Task? _screen;

        private bool _runing;

        public static BlockTextureCollection BlockTextureCollection { get; private set; }

        public static BdfFont DefaultFont { get; private set; }

        public static IReadOnlyDictionary<string, BdfFont> FontList => _fonts;

        public bool Runing => _runing;

        public bool EnableAccelerationEngine { get; }

        public TimeSpan SystemRunningTime => _stopwatch.Elapsed;

        public TimeSpan FrameMinTime { get; set; }

        public TimeSpan PreviousFrameTime { get; private set; }

        public TimeSpan NextFrameTime { get; private set; }

        public int FrameCount { get; private set; }

        public SystemTimer Timer { get; }

        public string Operator { get; set; }

        public Point CurrentPosition { get; private set; }

        public CursorType CursorType { get; set; }

        public MinecraftServer MinecraftServer { get; }

        public AccelerationEngine AccelerationEngine { get; }

        public ScreenManager ScreenManager { get; }

        public ProcessManager ProcessManager { get; }

        public FormManager FormManager { get; }

        public IReadOnlyDictionary<string, ApplicationInfo> ApplicationList => _apps;

        public string ServicesAppID { get; }

        public IReadOnlyList<string> StartupChecklist { get; }

        public void Start()
        {
            _runing = true;

            AccelerationEngine.Start();
            ScreenManager.ScreenContexts.Add(new(new(440, 206, -90), Facing.Xm, Facing.Ym, 256, 144));

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

        public Process? ProcessOf(Application application)
        {
            if (application is null)
                throw new ArgumentNullException(nameof(application));

            foreach (var process in ProcessManager.Process.Values)
                if (application == process.Application)
                    return process;

            return null;
        }

        public Process? ProcessOf(IForm form)
        {
            if (form is null)
                throw new ArgumentNullException(nameof(form));

            foreach (var process in ProcessManager.Process.Values)
                foreach (var activeForm in process.Application.FormManager.Forms)
                    if (activeForm == form)
                        return process;

            return null;
        }

        public ScreenContext? ScreenContextOf(IForm form)
        {
            if (form is null)
                throw new ArgumentNullException(nameof(form));

            foreach (var context in ScreenManager.ScreenContexts.Values)
                if (context.RootForm == form || context.RootForm.ContainsForm(form))
                    return context;

            return null;
        }

        public ScreenContext CreateScreenContext(Screen screen)
        {
            if (screen is null)
                throw new ArgumentNullException(nameof(screen));

            Process process = RunServicesApp();
            IRootForm rootForm = ((ServicesApplication)process.Application).RootForm;
            rootForm.RenderingSize = screen.Size;
            RunStartupChecklist(rootForm);
            return new(screen, rootForm);
        }

        public void ClearScreenContext(ScreenContext screentContext)
        {
            if (screentContext is null)
                throw new ArgumentNullException(nameof(screentContext));

            //TODO
        }

        private Process RunServicesApp()
        {
            if (!_apps[ServicesAppID].TypeObject.IsSubclassOf(typeof(ServicesApplication)))
                throw new InvalidOperationException("无效的ServicesAppID");

            return ProcessManager.Process.Add(_apps[ServicesAppID]);
        }

        private void RunStartupChecklist(IForm? initiator = null)
        {
            foreach (var id in StartupChecklist)
                ProcessManager.Process.Add(_apps[id], initiator);
        }

        public void RegisterApp(ApplicationInfo appInfo)
        {
            if (appInfo is null)
                throw new ArgumentNullException(nameof(appInfo));

            string dir = Path.Combine(PathManager.Applications_Dir, appInfo.ID);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            _apps.Add(appInfo.ID, appInfo);
        }

        public static void RegisterFont(string id, BdfFont font)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException($"“{nameof(id)}”不能为 null 或空。", nameof(id));
            if (font is null)
                throw new ArgumentNullException(nameof(font));

            _fonts.Add(id, font);
        }

        public static MCOS GetMCOS()
        {
            if (_mcos is null)
                throw new InvalidOperationException();
            return _mcos;
        }
    }
}
