using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class ProcessManager
    {
        public ProcessManager()
        {
            ProcessList = new(this);
            AddedProcess += OnAddedProcess;
            RemovedProcess += OnRemovedProcess;
        }

        public ProcessCollection ProcessList { get; }

        public event EventHandler<ProcessManager, ProcessEventArgs> AddedProcess;

        public event EventHandler<ProcessManager, ProcessEventArgs> RemovedProcess;

        protected virtual void OnAddedProcess(ProcessManager sender, ProcessEventArgs e) { }

        protected virtual void OnRemovedProcess(ProcessManager sender, ProcessEventArgs e) { }

        public void ProcessScheduling()
        {
            foreach (var process in ProcessList.ToArray())
            {
                process.Value.Handle();
                if (process.Value.ProcessState == ProcessState.Stopped)
                    ProcessList.Remove(process.Key);
            }
        }

        public class ProcessCollection : IDictionary<int, Process>
        {
            public ProcessCollection(ProcessManager owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
                _items = new();
                _id = 0;
            }

            private readonly ProcessManager _owner;

            private readonly Dictionary<int, Process> _items;

            private int _id;

            public Process this[int id] => _items[id];

            Process IDictionary<int, Process>.this[int key] { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public ICollection<int> Keys => _items.Keys;

            public ICollection<Process> Values => _items.Values;

            public int Count => _items.Count;

            public bool IsReadOnly => false;

            public Process Add(ApplicationInfo applicationInfo, IForm? initiator = null)
            {
                return Add(applicationInfo, Array.Empty<string>(), initiator);
            }

            public Process Add(ApplicationInfo applicationInfo, string[] args, IForm? initiator = null)
            {
                if (applicationInfo is null)
                    throw new ArgumentNullException(nameof(applicationInfo));
                if (args is null)
                    throw new ArgumentNullException(nameof(args));

                int id = _id;
                Process process = new(applicationInfo, args, initiator);
                process.ID = id;
                _id++;
                _items.Add(id, process);
                _owner.AddedProcess.Invoke(_owner, new(process));
                return process;
            }

            public bool Remove(int id)
            {
                if (!_items.TryGetValue(id, out var process) || !_items.Remove(id))
                    return false;

                process.ID = -1;
                _owner.RemovedProcess.Invoke(_owner, new(process));
                return true;
            }

            public void Clear()
            {
                foreach (var id in _items.Keys)
                    Remove(id);
            }

            public bool TryGetValue(int id, [MaybeNullWhen(false)] out Process process)
            {
                return _items.TryGetValue(id, out process);
            }

            public bool ContainsKey(int id)
            {
                return _items.ContainsKey(id);
            }

            public IEnumerator<KeyValuePair<int, Process>> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_items).GetEnumerator();
            }

            void ICollection<KeyValuePair<int, Process>>.Add(KeyValuePair<int, Process> item)
            {
                ((ICollection<KeyValuePair<int, Process>>)_items).Add(item);
            }

            bool ICollection<KeyValuePair<int, Process>>.Remove(KeyValuePair<int, Process> item)
            {
                return ((ICollection<KeyValuePair<int, Process>>)_items).Remove(item);
            }

            bool ICollection<KeyValuePair<int, Process>>.Contains(KeyValuePair<int, Process> item)
            {
                return ((ICollection<KeyValuePair<int, Process>>)_items).Contains(item);
            }

            void ICollection<KeyValuePair<int, Process>>.CopyTo(KeyValuePair<int, Process>[] array, int arrayIndex)
            {
                ((ICollection<KeyValuePair<int, Process>>)_items).CopyTo(array, arrayIndex);
            }

            void IDictionary<int, Process>.Add(int key, Process value)
            {
                throw new NotSupportedException();
            }
        }
    }
}
