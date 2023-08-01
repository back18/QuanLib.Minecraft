using Newtonsoft.Json.Linq;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Frame;
using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Screens
{
    public class ScreenManager
    {
        public ScreenManager()
        {
            ScreenConstructor = new();
            ScreenList = new(this);

            AddedScreen += OnAddedScreen;
            RemovedScreen += OnRemovedScreen;
        }

        public ScreenConstructor ScreenConstructor { get; }

        public ScreenCollection ScreenList { get; }

        public event EventHandler<ScreenManager, ScreenContextEventArgs> AddedScreen;

        public event EventHandler<ScreenManager, ScreenContextEventArgs> RemovedScreen;

        protected virtual void OnAddedScreen(ScreenManager sender, ScreenContextEventArgs e)
        {
            e.ScreenContext.Screen.Start();
        }

        protected virtual void OnRemovedScreen(ScreenManager sender, ScreenContextEventArgs e)
        {
            e.ScreenContext.Screen.Stop();
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
                tasks.Add((context.Key, Task.Run(() =>
                {
                    ArrayFrame frame = ArrayFrame.BuildFrame(context.Value.Screen.Width, context.Value.Screen.Height, context.Value.Screen.DefaultBackgroundBlcokID);
                    ArrayFrame? formFrame = UIRenderer.Rendering(context.Value.RootForm);
                    if (formFrame is not null)
                        frame.Overwrite(formFrame, context.Value.RootForm.RenderingLocation);
                    if (!SystemResourcesManager.CursorManager.TryGetValue(context.Value.CursorType, out var cursor))
                        cursor = SystemResourcesManager.CursorManager[CursorType.Default];
                    frame.Overwrite(cursor.Frame, context.Value.Screen.InputHandler.CurrentPosition, cursor.Offset);
                    return frame;
                })));
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

            private readonly Dictionary<int, ScreenContext> _items;

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

                int id = _id;
                ScreenContext context = MCOS.GetMCOS().CreateScreenContext(screen);
                context.ID = id;
                _items.Add(id, context);
                _owner.AddedScreen.Invoke(_owner, new(context));
                _id++;
                return context;
            }

            public bool Remove(int id)
            {
                if (!_items.TryGetValue(id, out var context) || !_items.Remove(id))
                    return false;

                context.ID = -1;
                _owner.RemovedScreen.Invoke(_owner, new(context));
                return true;
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
