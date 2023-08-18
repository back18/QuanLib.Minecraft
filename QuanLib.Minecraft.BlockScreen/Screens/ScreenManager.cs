#define TryCatch

using log4net.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuanLib.Minecraft.BlockScreen.Config;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Frame;
using QuanLib.Minecraft.BlockScreen.Logging;
using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Screens
{
    public class ScreenManager
    {
        private static readonly LogImpl LOGGER = LogUtil.MainLogger;

        public ScreenManager()
        {
            ScreenBuilder = new();
            ScreenList = new(this);

            _saves = new();

            AddedScreen += OnAddedScreen;
            RemovedScreen += OnRemovedScreen;
        }

        private readonly List<ScreenOptions> _saves;

        public ScreenBuilder ScreenBuilder { get; }

        public ScreenCollection ScreenList { get; }

        public event EventHandler<ScreenManager, ScreenContextEventArgs> AddedScreen;

        public event EventHandler<ScreenManager, ScreenContextEventArgs> RemovedScreen;

        protected virtual void OnAddedScreen(ScreenManager sender, ScreenContextEventArgs e) { }

        protected virtual void OnRemovedScreen(ScreenManager sender, ScreenContextEventArgs e) { }

        public void Initialize()
        {
            LOGGER.Info($"开始加载常驻屏幕，共计{ConfigManager.ScreenConfig.ResidentScreenList.Count}个");
            foreach (var options in ConfigManager.ScreenConfig.ResidentScreenList)
            {
#if TryCatch
                try
                {
#endif
                    ScreenList.Add(new(options)).LoadScreen();
#if TryCatch
                }
                catch (Exception ex)
                {
                    LOGGER.Error($"屏幕“{options}”无法加载", ex);
                }
#endif
            }

            ReadScreens();
            IEnumerable<ScreenOptions> creates = creates = _saves.Where(save => !ScreenList.Values.Any(context => context.Screen.EqualsScreenOption(save)));

            LOGGER.Info($"开始加载上次未关闭屏幕，共计{creates.Count()}个");
            foreach (var options in creates)
            {
#if TryCatch
                try
                {
#endif
                    ScreenList.Add(new(options)).LoadScreen();
#if TryCatch
                }
                catch (Exception ex)
                {
                    LOGGER.Error($"屏幕“{options}”无法加载", ex);
                }
#endif
            }
        }

        public void ScreenScheduling()
        {
            foreach (var context in ScreenList)
            {
                if (context.Value.ScreenState == ScreenState.Loading)
                {
                    if (!_saves.ContainsScreenOption(context.Value.Screen))
                        _saves.Add(new(context.Value.Screen));
                    SaveScreens();

                }
                context.Value.Handle();
                if (context.Value.ScreenState == ScreenState.Closed)
                {
                    ScreenList.Remove(context.Key);
                    _saves.Remove(new(context.Value.Screen));
                    SaveScreens();
                    if (context.Value.IsRestart)
                        ScreenList.Add(new(context.Value.Screen));
                }
            }
        }

        private void ReadScreens()
        {
            if (!File.Exists(MCOS.MainDirectory.Saves.ScreenSaves))
                return;

            string json = File.ReadAllText(MCOS.MainDirectory.Saves.ScreenSaves);
            ScreenOptions.Json[] items = JsonConvert.DeserializeObject<ScreenOptions.Json[]>(json) ?? throw new FormatException();
            foreach (var item in items)
            {
                ScreenOptions options = new(item);
                if (!_saves.ContainsScreenOption(options))
                    _saves.Add(options);
            }
        }

        private void SaveScreens()
        {
            List<ScreenOptions.Json> items = new();
            foreach (var save in _saves)
                items.Add(save.ToJson());
            string json = JsonConvert.SerializeObject(items);
            File.WriteAllText(MCOS.MainDirectory.Saves.ScreenSaves, json);
        }

        public void HandleAllScreenInput()
        {
            List<Task> tasks = new();
            foreach (var screen in ScreenList.Values)
                tasks.Add(Task.Run(() => screen.Screen.InputHandler.HandleInput()));
            Task.WaitAll(tasks.ToArray());
        }

        public void HandleAllBeforeFrame()
        {
            List<Task> tasks = new();
            foreach (var screen in ScreenList.Values)
                tasks.Add(Task.Run(() => screen.RootForm.HandleBeforeFrame(EventArgs.Empty)));
            Task.WaitAll(tasks.ToArray());
        }

        public void HandleAllAfterFrame()
        {
            List<Task> tasks = new();
            foreach (var screen in ScreenList.Values)
                tasks.Add(Task.Run(() => screen.RootForm.HandleAfterFrame(EventArgs.Empty)));
            Task.WaitAll(tasks.ToArray());
        }

        public void HandleAllUIRendering(out Dictionary<int, ArrayFrame> frames)
        {
            frames = new();
            List<(int id, Task<ArrayFrame> task)> tasks = new();
            foreach (var context in ScreenList)
            {
                if (context.Value.ScreenState == ScreenState.Closed)
                    continue;
                tasks.Add((context.Key, Task.Run(() =>
                {
                    ArrayFrame frame = ArrayFrame.BuildFrame(context.Value.Screen.Width, context.Value.Screen.Height, context.Value.Screen.DefaultBackgroundBlcokID);
                    ArrayFrame? formFrame = UIRenderer.Rendering(context.Value.RootForm);
                    if (formFrame is not null)
                        frame.Overwrite(formFrame, context.Value.RootForm.ClientLocation);
                    if (context.Value.IsShowCursor)
                    {
                        if (!SystemResourcesManager.CursorManager.TryGetValue(context.Value.CursorType, out var cursor))
                            cursor = SystemResourcesManager.CursorManager[CursorType.Default];
                        frame.Overwrite(cursor.Frame, context.Value.Screen.InputHandler.CurrentPosition, cursor.Offset);
                    }
                    return frame;
                })));
            }
            Task.WaitAll(tasks.Select(i => i.task).ToArray());
            foreach (var (id, task) in tasks)
                frames.Add(id, task.Result);
        }

        public async Task HandleAllScreenOutputAsync(Dictionary<int, ArrayFrame> frames)
        {
            if (frames is null)
                throw new ArgumentNullException(nameof(frames));

            List<Task> tasks = new();
            foreach (var frame in frames)
            {
                if (ScreenList.TryGetValue(frame.Key, out var context))
                    tasks.Add(context.Screen.OutputHandler.HandleOutputAsync(frame.Value));
            }
            await Task.WhenAll(tasks);
        }

        public void WaitAllScreenPrevious()
        {
            List<Task> tasks = new();
            foreach (var screen in ScreenList.Values)
                tasks.Add(screen.Screen.OutputHandler.WaitPreviousAsync());
            Task.WaitAll(tasks.ToArray());
        }

        public class ScreenCollection : IDictionary<int, ScreenContext>
        {
            public ScreenCollection(ScreenManager owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
                _items = new();
                _id = 0;
            }

            private readonly ScreenManager _owner;

            private readonly ConcurrentDictionary<int, ScreenContext> _items;

            private int _id;

            public ICollection<int> Keys => _items.Keys;

            public ICollection<ScreenContext> Values => _items.Values;

            public int Count => _items.Count;

            public bool IsReadOnly => false;

            public ScreenContext this[int id] => _items[id];

            ScreenContext IDictionary<int, ScreenContext>.this[int key] { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public ScreenContext Add(Screen screen)
            {
                if (screen is null)
                    throw new ArgumentNullException(nameof(screen));

                lock (this)
                {
                    foreach (var value in Values)
                    {
                        if (value.Screen.EqualsScreenOption(screen))
                            throw new ArgumentException("尝试加载相同的屏幕" ,nameof(screen));
                    }

                    int id = _id;
                    Process process = MCOS.Instance.RunServicesApp();
                    IRootForm rootForm = ((ServicesApplication)process.Application).RootForm;
                    ScreenContext context = new(screen, rootForm);
                    context.ID = id;
                    _items.TryAdd(id, context);
                    _owner.AddedScreen.Invoke(_owner, new(context));
                    _id++;
                    return context;
                }
            }

            public bool Remove(int id)
            {
                lock (this)
                {
                    if (!_items.TryGetValue(id, out var context) || !_items.TryRemove(id, out _))
                        return false;

                    context.ID = -1;
                    _owner.RemovedScreen.Invoke(_owner, new(context));
                    return true;
                }
            }

            public void Clear()
            {
                foreach (var id in _items.Keys.ToArray())
                    Remove(id);
            }

            public bool ContainsKey(int id)
            {
                return _items.ContainsKey(id);
            }

            public bool TryGetValue(int id, [MaybeNullWhen(false)] out ScreenContext context)
            {
                return _items.TryGetValue(id, out context);
            }

            public IEnumerator<KeyValuePair<int, ScreenContext>> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_items).GetEnumerator();
            }

            void ICollection<KeyValuePair<int, ScreenContext>>.Add(KeyValuePair<int, ScreenContext> item)
            {
                ((ICollection<KeyValuePair<int, ScreenContext>>)_items).Add(item);
            }

            bool ICollection<KeyValuePair<int, ScreenContext>>.Remove(KeyValuePair<int, ScreenContext> item)
            {
                return ((ICollection<KeyValuePair<int, ScreenContext>>)_items).Remove(item);
            }

            bool ICollection<KeyValuePair<int, ScreenContext>>.Contains(KeyValuePair<int, ScreenContext> item)
            {
                return ((ICollection<KeyValuePair<int, ScreenContext>>)_items).Contains(item);
            }

            void ICollection<KeyValuePair<int, ScreenContext>>.CopyTo(KeyValuePair<int, ScreenContext>[] array, int arrayIndex)
            {
                ((ICollection<KeyValuePair<int, ScreenContext>>)_items).CopyTo(array, arrayIndex);
            }

            void IDictionary<int, ScreenContext>.Add(int key, ScreenContext value)
            {
                throw new NotSupportedException();
            }
        }
    }
}
