using log4net.Core;
using Newtonsoft.Json;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Logging;
using QuanLib.Minecraft.Selectors;
using QuanLib.Minecraft.Snbt.Data;
using QuanLib.Minecraft.Vector;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class InteractionManager
    {
        private static readonly LogImpl LOGGER = LogUtil.MainLogger;

        public InteractionManager()
        {
            Items = new(this);
            AddedInteraction += OnAddedInteraction;
            RemovedInteraction += OnRemovedInteraction;
        }

        public InteractionCollection Items { get; }

        public event EventHandler<InteractionManager, InteractionEventArgs> AddedInteraction;

        public event EventHandler<InteractionManager, InteractionEventArgs> RemovedInteraction;

        protected virtual void OnAddedInteraction(InteractionManager sender, InteractionEventArgs e) { }

        protected virtual void OnRemovedInteraction(InteractionManager sender, InteractionEventArgs e) { }

        public void Initialize()
        {
            string dir = MCOS.MainDirectory.Saves.Interactions.Directory;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string[] files = Directory.GetFiles(dir, "*.json");
            LOGGER.Info($"开始回收交互实体，共计{files.Length}个");
            foreach (string file in files)
            {
                try
                {
                    Interaction.Json json = JsonConvert.DeserializeObject<Interaction.Json>(File.ReadAllText(file)) ?? throw new FormatException();
                    EntityPos position = new(json.Position[0], json.Position[1], json.Position[2]);
                    BlockPos blockPos = position.ToBlockPos();
                    var command = MCOS.Instance.MinecraftServer.CommandHelper;
                    command.AddForceLoadChunk(blockPos);
                    command.KillEntity(new GenericSelector(json.EntityUUID));
                    command.RemoveForceLoadChunk(blockPos);
                    File.Delete(file);
                    LOGGER.Info($"玩家[{json.PlayerUUID}]的交互实体已回收");
                }
                catch (Exception ex)
                {
                    LOGGER.Error("无法回收交互实体", ex);
                }
            }
        }

        public void InteractionScheduling()
        {
            foreach (var item in Items)
            {
                item.Value.Handle();
                if (item.Value.InteractionState == InteractionState.Closed)
                    Items.Remove(item.Key);
            }
        }

        public class InteractionCollection : IDictionary<string, Interaction>
        {
            public InteractionCollection(InteractionManager owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
                _items = new();
            }

            private readonly InteractionManager _owner;

            private readonly ConcurrentDictionary<string, Interaction> _items;

            public Interaction this[string player] => _items[player];

            Interaction IDictionary<string, Interaction>.this[string key] { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public ICollection<string> Keys => _items.Keys;

            public ICollection<Interaction> Values => _items.Values;

            public int Count => _items.Count;

            public bool IsReadOnly => false;

            public bool TryAdd(string player, [MaybeNullWhen(false)] out Interaction interaction)
            {
                if (player is null)
                    throw new ArgumentNullException(nameof(player));

                lock (this)
                {
                    if (_items.ContainsKey(player))
                        goto err;

                    if (!Interaction.TryCreate(player, out interaction))
                        goto err;

                    _items.TryAdd(interaction.PlayerName, interaction);
                    _owner.AddedInteraction.Invoke(_owner, new(interaction));
                    return true;

                    err:
                    interaction = null;
                    return false;
                }
            }

            public bool Remove(string player)
            {
                lock (this)
                {
                    if (!_items.TryGetValue(player, out var interaction) || !_items.TryRemove(player, out _))
                        return false;

                    _owner.RemovedInteraction.Invoke(_owner, new(interaction));
                    return true;
                }
            }

            public void Clear()
            {
                foreach (var player in _items.Keys)
                    Remove(player);
            }

            public bool ContainsKey(string player)
            {
                return _items.ContainsKey(player);
            }

            public bool TryGetValue(string player, [MaybeNullWhen(false)] out Interaction interaction)
            {
                return _items.TryGetValue(player, out interaction);
            }

            public IEnumerator<KeyValuePair<string, Interaction>> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_items).GetEnumerator();
            }

            void ICollection<KeyValuePair<string, Interaction>>.Add(KeyValuePair<string, Interaction> item)
            {
                ((ICollection<KeyValuePair<string, Interaction>>)_items).Add(item);
            }

            bool ICollection<KeyValuePair<string, Interaction>>.Remove(KeyValuePair<string, Interaction> item)
            {
                return ((ICollection<KeyValuePair<string, Interaction>>)_items).Remove(item);
            }

            bool ICollection<KeyValuePair<string, Interaction>>.Contains(KeyValuePair<string, Interaction> item)
            {
                return ((ICollection<KeyValuePair<string, Interaction>>)_items).Contains(item);
            }

            void ICollection<KeyValuePair<string, Interaction>>.CopyTo(KeyValuePair<string, Interaction>[] array, int arrayIndex)
            {
                ((ICollection<KeyValuePair<string, Interaction>>)_items).CopyTo(array, arrayIndex);
            }

            void IDictionary<string, Interaction>.Add(string key, Interaction value)
            {
                throw new NotSupportedException();
            }
        }
    }
}
