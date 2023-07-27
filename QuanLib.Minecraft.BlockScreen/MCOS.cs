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

namespace QuanLib.Minecraft.BlockScreen
{
    public class MCOS : ISwitchable
    {
        static MCOS()
        {
            Task<McbsConfig> task1 = Task.Run(() => new McbsConfig(JsonConvert.DeserializeObject<McbsConfig.Json>(File.ReadAllText(Path.Combine(PathManager.Main_Dir, "MCBS.json"))) ?? throw new FormatException()));
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


            McbsConfig = task1.Result;
            BlockTextureCollection = task2.Result;
            DefaultFont = task3.Result;
            _cursors = task4.Result;
            task5.Wait();

            _fonts = new();
            RegisterFont("DefaultFont", DefaultFont);

            ControlRenderer = new ControlRenderer();
        }

        public MCOS(
            MinecraftServer minecraftServer,
            Screen screen,
            ScreenInputReader cursorReader,
            AccelerationEngine accelerationEngine)
        {
            MinecraftServer = minecraftServer ?? throw new ArgumentNullException(nameof(minecraftServer));
            Screen = screen ?? throw new ArgumentNullException(nameof(screen));
            ScreenInputReader = cursorReader ?? throw new ArgumentNullException(nameof(cursorReader));
            ScreenConstructor = new("snowball_mouse");
            AccelerationEngine = accelerationEngine ?? throw new ArgumentNullException(nameof(accelerationEngine));

            Screen.MCOS = this;
            ScreenInputReader.MCOS = this;
            ScreenConstructor.MCOS = this;

            EnableAccelerationEngine = true;
            FrameCount = 0;
            FrameMinTime = TimeSpan.FromMilliseconds(50);
            PreviousFrameTime = TimeSpan.Zero;
            NextFrameTime = PreviousFrameTime + FrameMinTime;
            Timer = new();
            Operator = string.Empty;
            CursorType = CursorType.Default;
            ServicesAppID = McbsConfig.ServicesAppID;
            StartupChecklist = McbsConfig.StartupChecklist;

            _apps = new();
            _process = new();
            _callbacks = new();
            _stopwatch = new();
        }

        private static readonly Dictionary<string, BdfFont> _fonts;

        private static readonly Dictionary<CursorType, Cursor> _cursors;

        private readonly Dictionary<string, ApplicationInfo> _apps;

        private readonly Dictionary<string, Process> _process;

        internal readonly Queue<Action> _callbacks;

        private readonly Stopwatch _stopwatch;

        private Task? _screen;

        private bool _runing;

        public static McbsConfig McbsConfig { get; }

        public static BlockTextureCollection BlockTextureCollection { get; private set; }

        public static BdfFont DefaultFont { get; private set; }

        public static IReadOnlyDictionary<string, BdfFont> FontList => _fonts;

        public static ControlRenderer ControlRenderer { get; }

        public bool Runing => _runing;

        public bool EnableAccelerationEngine { get; }

        public TimeSpan SystemRunningTime => _stopwatch.Elapsed;

        public TimeSpan FrameMinTime { get; set; }

        public TimeSpan PreviousFrameTime { get; private set; }

        public TimeSpan NextFrameTime { get; private set; }

        public int FrameCount { get; private set; }

        public SystemTimer Timer { get; }

        public Size FormsPanelSize => RootForm.FormsPanelClientSize;

        public string Operator { get; set; }

        public Point CurrentPosition { get; private set; }

        public CursorType CursorType { get; set; }

        public MinecraftServer MinecraftServer { get; }

        public Screen Screen { get; }

        public AccelerationEngine AccelerationEngine { get; }

        public ScreenInputReader ScreenInputReader { get; }

        public ScreenConstructor ScreenConstructor { get; }

        public IReadOnlyDictionary<string, ApplicationInfo> ApplicationList => _apps;

        public IReadOnlyDictionary<string, Process> ProcessList => _process;

        public string ServicesAppID { get; }

        public IReadOnlyList<string> StartupChecklist { get; }

        public ApplicationInfo ServicesAppInfo => _apps[ServicesAppID];

        public Process ServicesProcess => _process[ServicesAppID];

        public ServicesApplication ServicesApp => (ServicesApplication)ServicesProcess.Application;

        public IRootForm RootForm => ServicesApp.RootForm;

        public void Initialize()
        {
            ScreenInputReader.OnCursorMove += (Point position) =>
            {
                CurrentPosition = position;
                RootForm.HandleCursorMove(position);
            };

            ScreenInputReader.OnRightClick += (Point position) => RootForm.HandleRightClick(position);

            ScreenInputReader.OnLeftClick += (Point position) => RootForm.HandleLeftClick(position);

            ScreenInputReader.OnTextUpdate += (Point position, string text) => RootForm.HandleTextEditorUpdate(position, text);
        }

        public void Start()
        {
            _runing = true;

            RunServicesApp();
            RunStartupChecklist();
            AccelerationEngine.Start();

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

                if (_screen?.IsCompleted ?? true)
                {
                    HandleCursorEvent();
                }
                else
                {
                    _callbacks.Enqueue(() => HandleCursorEvent());
                    lags++;
                }
                ScreenConstructor.Handle();
                HandleBeforeFrame();
                HandleRenderingFrame(out var frame);
                HandleUpdateScreen(frame);
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

            foreach (var process in _process.ToArray())
            {
                if (process.Key == ServicesAppInfo.ID)
                    continue;

                foreach (var active in process.Value.Application.ActiveForms)
                {
                    switch (process.Value.ProcessState)
                    {
                        case ProcessState.Running:
                            if (!RootForm.ContainsForm(active))
                            {
                                RootForm.AddForm(active);
                            }
                            break;
                        case ProcessState.Pending:
                            if (RootForm.ContainsForm(active))
                            {
                                RootForm.RemoveForm(active);
                            }
                            break;
                        case ProcessState.Stopped:
                            if (RootForm.ContainsForm(active))
                            {
                                RootForm.RemoveForm(active);
                            }
                            break;
                    }
                }
            }

            stopwatch.Stop();
            Timer.ProcessScheduling.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleCursorEvent()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ScreenInputReader.Handle();

            stopwatch.Stop();
            Timer.CursorEvent.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleBeforeFrame()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            RootForm.HandleBeforeFrame();

            stopwatch.Stop();
            Timer.HandleBeforeFrame.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        public TimeSpan HandleAfterFrame()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            RootForm.HandleAfterFrame();

            stopwatch.Stop();
            Timer.HandleAfterFrame.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleRenderingFrame(out ArrayFrame frame)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            frame = ArrayFrame.BuildFrame(Screen.Width, Screen.Height, Screen.DefaultBackgroundBlcokID);
            ArrayFrame? formFrame = ControlRenderer.Rendering(RootForm);
            if (formFrame is not null)
                frame.Overwrite(formFrame, RootForm.RenderingLocation);
            frame.Overwrite(_cursors[CursorType].Frame, CurrentPosition, _cursors[CursorType].Offset);

            stopwatch.Stop();
            Timer.RenderingFrame.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleUpdateScreen(ArrayFrame frame)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Task? previous = _screen;
            _screen = Screen.ShowNewFrameAsync(frame, previous);
            previous?.Wait();

            stopwatch.Stop();
            Timer.UpdateScreen.Add(stopwatch.Elapsed);
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

        private void RunApp(ApplicationInfo appInfo, string[] args, Process? initiator = null)
        {
            Process process = new(appInfo, args, initiator);
            process.MCOS = this;
            process.Application.MCOS = this;
            process.Application.Process = process;
            process.OnStopped += Process_OnStopped;
            _process.Add(process.ApplicationInfo.ID, process);
            process.Application.Initialize();
            process.MainThread.Start();
        }

        public void RunApp(string appID, string[] args, Process? initiator = null)
        {
            if (appID is null)
                throw new ArgumentNullException(nameof(appID));

            if (_process.TryGetValue(appID, out var process))
            {
                process.IsPending = false;
            }
            else if (_apps.TryGetValue(appID, out var appInfo))
            {
                RunApp(appInfo, args, initiator);
            }
            else
            {
                throw new ArgumentException("未知的AppID", nameof(appID));
            }
        }

        public void RunApp(string appID, Process? initiator = null)
        {
            RunApp(appID, Array.Empty<string>(), initiator);
        }

        private void RunServicesApp()
        {
            if (!_apps[ServicesAppID].TypeObject.IsSubclassOf(typeof(ServicesApplication)))
                throw new InvalidOperationException("无效的ServicesAppID");

            RunApp(ServicesAppID);
        }

        private void RunStartupChecklist()
        {
            foreach (var item in StartupChecklist)
                RunApp(item);
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

        private void Process_OnStopped(Process process)
        {
            _process.Remove(process.ApplicationInfo.ID);
        }

        public static void RegisterFont(string id, BdfFont font)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException($"“{nameof(id)}”不能为 null 或空。", nameof(id));
            if (font is null)
                throw new ArgumentNullException(nameof(font));

            _fonts.Add(id, font);
        }
    }
}
