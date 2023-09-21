using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack.Block
{
    public class BlockTextureManager : IReadOnlyDictionary<string, BlockTexture>
    {
        static BlockTextureManager()
        {
            _slock = new();
            IsLoaded = false;
        }

        internal BlockTextureManager(Dictionary<string, BlockTexture> items)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));

            _map = new()
            {
                { Facing.Xp, new() },
                { Facing.Xm, new() },
                { Facing.Yp, new() },
                { Facing.Ym, new() },
                { Facing.Zp, new() },
                { Facing.Zm, new() }
            };
            foreach (var texture in _items.Values)
            {
                if (texture.BlockType == BlockType.CubeAll)
                {
                    _map[Facing.Xp][texture.AverageColors[Facing.Xp]] = texture;
                    _map[Facing.Xm][texture.AverageColors[Facing.Xm]] = texture;
                    _map[Facing.Yp][texture.AverageColors[Facing.Yp]] = texture;
                    _map[Facing.Ym][texture.AverageColors[Facing.Ym]] = texture;
                    _map[Facing.Zp][texture.AverageColors[Facing.Zp]] = texture;
                    _map[Facing.Zm][texture.AverageColors[Facing.Zm]] = texture;
                }
                else
                {
                    _map[Facing.Xp].TryAdd(texture.AverageColors[Facing.Xp], texture);
                    _map[Facing.Xm].TryAdd(texture.AverageColors[Facing.Xm], texture);
                    _map[Facing.Yp].TryAdd(texture.AverageColors[Facing.Yp], texture);
                    _map[Facing.Ym].TryAdd(texture.AverageColors[Facing.Ym], texture);
                    _map[Facing.Zp].TryAdd(texture.AverageColors[Facing.Zp], texture);
                    _map[Facing.Zm].TryAdd(texture.AverageColors[Facing.Zm], texture);
                }
            }
        }

        private static readonly object _slock;

        public static bool IsLoaded { get; private set; }

        public static BlockTextureManager Instance
        {
            get
            {
                if (_Instance is null)
                    throw new InvalidOperationException("实例未加载");
                return _Instance;
            }
        }
        private static BlockTextureManager? _Instance;

        private readonly Dictionary<string, BlockTexture> _items;

        private readonly Dictionary<Facing, Dictionary<Rgba32, BlockTexture>> _map;

        public BlockTexture this[string index] => _items[index];

        public IEnumerable<string> Keys => _items.Keys;

        public IEnumerable<BlockTexture> Values => _items.Values;

        public int Count => _items.Count;

        public static BlockTextureManager LoadInstance(ResourceEntryManager resources, IEnumerable<BlockState> blacklist)
        {
            if (resources is null)
                throw new ArgumentNullException(nameof(resources));
            if (blacklist is null)
                throw new ArgumentNullException(nameof(blacklist));

            lock (_slock)
            {
                if (_Instance is not null)
                    throw new InvalidOperationException("试图重复加载单例实例");

                _Instance = BlockTextureReader.Load(resources, blacklist);
                IsLoaded = true;
                return _Instance;
            }
        }

        public BlockTexture? MatchBlockTexture(Facing facing, Rgba32 rgba32)
        {
            if (_map[facing].TryGetValue(rgba32, out BlockTexture? result))
                return result;
            Vector4 vector4 = rgba32.ToVector4();
            float distance = float.MaxValue;
            foreach (var texture in _items.Values)
            {
                float newDistance = Vector4.DistanceSquared(vector4, texture.AverageColors[facing].ToVector4());
                if (newDistance < distance)
                {
                    distance = newDistance;
                    result = texture;
                }
            }

            if (result is not null)
            {
                lock (_map[facing])
                    _map[facing].TryAdd(rgba32, result);
            }
            return result;
        }

        public BlockTexture? MatchBlockTexture<T>(Facing facing, T pixel) where T : IPixel
        {
            Rgba32 rgba32 = new();
            pixel.ToRgba32(ref rgba32);
            return MatchBlockTexture(facing, rgba32);
        }

        public void BuildMapCache(Facing facing)
        {
            ConcurrentDictionary<Rgba32, BlockTexture> result = new();

            Rgba32 rgba = new(0, 0, 0, 255);
            int max = 256 * 256 * 256;
            int count = 0;
            Parallel.For(0, max, (i) =>
            {
                if (TryGetNextRgba32(out var next))
                {
                    BlockTexture? temp = null;
                    Vector4 vector4 = next.ToVector4();
                    float distance = float.MaxValue;
                    foreach (var texture in _items.Values)
                    {
                        float newDistance = Vector4.DistanceSquared(vector4, texture.AverageColors[facing].ToVector4());
                        if (newDistance < distance)
                        {
                            distance = newDistance;
                            temp = texture;
                        }
                    }
                    if (temp is not null)
                        result.TryAdd(next, temp);
                }
                Interlocked.Increment(ref count);
            });

            _map[facing] = result.ToDictionary(kv => kv.Key, kv => kv.Value);

            while (count < max)
                Thread.Sleep(10);

            bool TryGetNextRgba32(out Rgba32 next)
            {
                lock (result)
                {
                    if (rgba.R < 255)
                    {
                        rgba.R++;
                        next = rgba;
                        return true;
                    }
                    else if (rgba.G < 255)
                    {
                        rgba.R = 0;
                        rgba.G++;
                        next = rgba;
                        return true;
                    }
                    else if (rgba.B < 255)
                    {
                        rgba.R = 0;
                        rgba.G = 0;
                        rgba.B++;
                        next = rgba;
                        return true;
                    }
                    else
                    {
                        next = rgba = default;
                        return false;
                    }
                }
            }
        }

        public bool ContainsKey(string key)
        {
            return _items.ContainsKey(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out BlockTexture value)
        {
            if (_items.TryGetValue(key, out value))
                return true;

            foreach (var item in _items)
            {
                if (item.Key.StartsWith(key))
                {
                    value = item.Value;
                    return true;
                }
            }

            return false;
        }

        public IEnumerator<KeyValuePair<string, BlockTexture>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }
}
