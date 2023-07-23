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
using QuanLib.Minecraft.BlockScreen.BuiltInApps.Desktop;
using QuanLib.Minecraft.BlockScreen.BuiltInApps.Services;
using QuanLib.BDF;
using SixLabors.ImageSharp;
using FFMediaToolkit;
using QuanLib.Minecraft.BlockScreen.UI.Controls;
using QuanLib.Minecraft.BlockScreen.UI;

namespace QuanLib.Minecraft.BlockScreen
{
    public class MCOS : ISwitchable
    {
        static MCOS()
        {
            Task<BdfFont> task1 = Task.Run(() => BdfFont.Load(Path.Combine(PathManager.SystemResources_Fonts_Dir, "DefaultFont.bdf")));
            Task task2 = Task.Run(() =>
            {
                FFmpegLoader.FFmpegPath = PathManager.FFmpeg_Dir;
                FFmpegLoader.LoadFFmpeg();
            });
            Task<BlockTextureCollection> task3 = Task.Run(() => BlockTextureCollection.Load(Path.Combine(PathManager.MinecraftResources_Dir, "assets", "minecraft")));
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

            DefaultFont = task1.Result;
            task2.Wait();
            BlockTextureCollection = task3.Result;
            _cursors = task4.Result;

            _fonts = new();
            RegisterFont("DefaultFont", DefaultFont);

            UIRenderer = new ControlRenderer();
        }

        public MCOS(
            MinecraftServer minecraftServer,
            Screen screen,
            AccelerationEngine ae,
            PlayerCursorReader cursorReader)
        {
            MinecraftServer = minecraftServer ?? throw new ArgumentNullException(nameof(minecraftServer));
            Screen = screen ?? throw new ArgumentNullException(nameof(screen));
            AccelerationEngine = ae ?? throw new ArgumentNullException(nameof(ae));
            PlayerCursorReader = cursorReader ?? throw new ArgumentNullException(nameof(cursorReader));
            GroupTextReader = new();

            Screen.MCOS = this;
            PlayerCursorReader.MCOS = this;
            GroupTextReader.MCOS = this;

            EnableAccelerationEngine = true;
            FrameCount = 0;
            FrameMinTime = TimeSpan.FromMilliseconds(50);
            PreviousFrameTime = TimeSpan.Zero;
            NextFrameTime = PreviousFrameTime + FrameMinTime;
            Timer = new();
            ScreenDefaultBackgroundBlcokID = ConcretePixel.ToBlockID(MinecraftColor.LightBlue);
            Operator = string.Empty;
            CursorType = CursorType.Default;

            _apps = new();
            _process = new();
            _callbacks = new();
            _stopwatch = new();

            AccelerationEngine.Start();

            RegisterApp(new ServicesAppInfo());
            RegisterApp(new DesktopAppInfo());
        }

        private static readonly Dictionary<string, BdfFont> _fonts;

        private static readonly Dictionary<CursorType, Cursor> _cursors;

        private readonly Dictionary<string, ApplicationInfo> _apps;

        private readonly Dictionary<string, Process> _process;

        internal readonly Queue<Action> _callbacks;

        private readonly Stopwatch _stopwatch;

        private Task? _screen;

        private bool _runing;

        public static BlockTextureCollection BlockTextureCollection { get; private set; }

        public static BdfFont DefaultFont { get; private set; }

        public static IReadOnlyDictionary<string, BdfFont> FontList => _fonts;

        public static UIRenderer<IControlRendering> UIRenderer { get; }

        public bool Runing => _runing;

        public bool EnableAccelerationEngine { get; }

        public TimeSpan SystemRunningTime => _stopwatch.Elapsed;

        public TimeSpan FrameMinTime { get; set; }

        public TimeSpan PreviousFrameTime { get; private set; }

        public TimeSpan NextFrameTime { get; private set; }

        public int FrameCount { get; private set; }

        public SystemTimer Timer { get; }

        public string ScreenDefaultBackgroundBlcokID { get; set; }

        public Size FormsPanelSize => ServicesApp.RootForm.FormsPanel.ClientSize;

        public string Operator { get; set; }

        public Point CurrentPosition { get; private set; }

        public CursorType CursorType { get; set; }

        public MinecraftServer MinecraftServer { get; }

        public Screen Screen { get; }

        public AccelerationEngine AccelerationEngine { get; }

        public PlayerCursorReader PlayerCursorReader { get; }

        public GroupTextReader GroupTextReader { get; }

        public IReadOnlyDictionary<string, ApplicationInfo> ApplicationList => _apps;

        public IReadOnlyDictionary<string, Process> ProcessList => _process;

        public Process ServicesProcess => _process[ServicesApp.ID];

        public Process DesktopProcess => _process[DesktopApp.ID];

        public ServicesApp ServicesApp => (ServicesApp)ServicesProcess.Application;

        public DesktopApp DesktopApp => (DesktopApp)DesktopProcess.Application;

        public void Initialize()
        {
            PlayerCursorReader.OnCursorMove += (Point position, CursorMode mode) =>
            {
                CurrentPosition = position;
                ServicesApp.RootForm.HandleCursorMove(position, mode);
            };

            PlayerCursorReader.OnRightClick += (Point position) => ServicesApp.RootForm.HandleRightClick(position);

            PlayerCursorReader.OnLeftClick += (Point position) => ServicesApp.RootForm.HandleLeftClick(position);

            PlayerCursorReader.OnTextUpdate += (Point position, string text) => ServicesApp.RootForm.HandleTextEditorUpdate(position, text);
        }

        public void Start()
        {
            _runing = true;

            RunApp(ServicesApp.ID, Array.Empty<string>());
            RunApp(DesktopApp.ID, Array.Empty<string>(), ServicesProcess);

            Console.CursorVisible = false;
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

                HandleBeforeFrame();
                HandleRenderingFrame(out var frame);
                HandleUpdateScreen(frame);
                HandleAfterFrame();
                HandleSystemInterrupt();

                Timer.TotalTime.Add(SystemRunningTime - PreviousFrameTime);

                string empty = new(' ', 200);
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < 10; i++)
                    Console.WriteLine(empty);
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(Timer.ToString(BlockScreen.Timer.Duration.Tick20));
                Console.WriteLine($"帧: {FrameCount}");
                Console.WriteLine($"滞后: {lags}");
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
                var forms = ServicesApp.RootForm.FormsPanel.SubControls;
                if (process.Key != ServicesApp.AppID)
                {
                    Form form = process.Value.Application.ForegroundForm;
                    switch (process.Value.ProcessState)
                    {
                        case ProcessState.Running:
                            if (!forms.Contains(form))
                            {
                                forms.Add(form);
                                ServicesApp.RootForm.TrySwitchForm(form);
                            }
                            break;
                        case ProcessState.Pending:
                            if (ServicesApp.RootForm.SubControls.Contains(form))
                            {
                                form.IsSelected = false;
                                forms.Remove(form);
                                ServicesApp.RootForm.SelectedMaxDisplayPriority();
                            }
                            break;
                        case ProcessState.Stopped:
                            if (forms.Contains(form))
                            {
                                _process.Remove(process.Key);
                                form.IsSelected = false;
                                forms.Remove(form);
                                ServicesApp.RootForm.SelectedMaxDisplayPriority();
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

            PlayerCursorReader.Handle();

            stopwatch.Stop();
            Timer.CursorEvent.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleBeforeFrame()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ServicesApp.ForegroundForm.HandleBeforeFrame();

            stopwatch.Stop();
            Timer.HandleBeforeFrame.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        public TimeSpan HandleAfterFrame()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ServicesApp.ForegroundForm.HandleAfterFrame();

            stopwatch.Stop();
            Timer.HandleAfterFrame.Add(stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }

        private TimeSpan HandleRenderingFrame(out ArrayFrame frame)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            frame = ArrayFrame.BuildFrame(Screen.Width, Screen.Height, ScreenDefaultBackgroundBlcokID);
            ArrayFrame? formFrame = UIRenderer.Rendering(ServicesApp.RootForm);
            if (formFrame is not null)
                frame.Overwrite(formFrame, ServicesApp.ForegroundForm.Location);
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
            _process.Add(process.Application.AppID, process);
            process.MCOS = this;
            process.Application.MCOS = this;
            process.Application.Process = process;
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
    }
}
