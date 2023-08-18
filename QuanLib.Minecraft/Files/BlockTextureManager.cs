using Newtonsoft.Json.Linq;
using QuanLib.Minecraft.DirectoryManagers;
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

namespace QuanLib.Minecraft.Files
{
    public class BlockTextureManager : IReadOnlyDictionary<string, BlockTexture>
    {
        private BlockTextureManager(Dictionary<string, BlockTexture> items)
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
                if (texture.TextureType == BlockTextureType.CubeAll)
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

        private readonly Dictionary<string, BlockTexture> _items;

        private readonly Dictionary<Facing, Dictionary<Rgba32, BlockTexture>> _map;

        public BlockTexture this[string index] => _items[index];

        public IEnumerable<string> Keys => _items.Keys;

        public IEnumerable<BlockTexture> Values => _items.Values;

        public int Count => _items.Count;

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

        public static BlockTextureManager LoadDirectory(string directory, IEnumerable<string> blacklist)
        {
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentException($"“{nameof(directory)}”不能为 null 或空。", nameof(directory));
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException(nameof(directory));

            ResourcePackDirectory resourcePack = new(directory);
            string[] blockStates = Directory.GetFiles(resourcePack.BlockStates, "*.json");
            Dictionary<string, JObject> modelsCache = new();
            Dictionary<string, Image<Rgba32>> imagesCache = new();
            Dictionary<string, BlockTexture> result = new();
            foreach (var blockState in blockStates)
            {
                JObject jobj1 = JObject.Parse(File.ReadAllText(blockState));
                if (jobj1.TryGetValue("variants", out var variants))
                {
                    foreach (var variant in variants)
                    {
                        if (variant is JProperty variant_jpro &&
                            variant_jpro.Value is JObject variant_jobj &&
                            variant_jobj.TryGetValue("model", out var model) &&
                            TryGetName(model, out var name))
                        {
                            JObject jobj2 = GetModel(name);
                            if (jobj2.TryGetValue("parent", out var parent) && jobj2.TryGetValue("textures", out var textures) && textures is JObject textures_jobj)
                            {
                                string? value = parent.Value<string>();
                                BlockTextureType type;
                                string xp, xm, yp, ym, zp, zm;
                                switch (value)
                                {
                                    case "minecraft:block/cube_all":
                                        {
                                            if (textures_jobj.TryGetValue("all", out var all) && TryGetName(all, out var allName))
                                            {
                                                xp = allName;
                                                xm = allName;
                                                yp = allName;
                                                ym = allName;
                                                zp = allName;
                                                zm = allName;
                                            }
                                            else
                                                continue;
                                            type = BlockTextureType.CubeAll;
                                            break;
                                        }

                                    case "minecraft:block/cube":
                                        {
                                            if (textures_jobj.TryGetValue("east", out var east) && TryGetName(east, out var eastName) &&
                                                textures_jobj.TryGetValue("west", out var west) && TryGetName(west, out var westName) &&
                                                textures_jobj.TryGetValue("up", out var up) && TryGetName(up, out var upName) &&
                                                textures_jobj.TryGetValue("down", out var down) && TryGetName(down, out var downName) &&
                                                textures_jobj.TryGetValue("south", out var south) && TryGetName(south, out var southName) &&
                                                textures_jobj.TryGetValue("north", out var north) && TryGetName(north, out var northName))
                                            {
                                                xp = eastName;
                                                xm = westName;
                                                yp = upName;
                                                ym = downName;
                                                zp = southName;
                                                zm = northName;
                                            }
                                            else
                                                continue;
                                            type = BlockTextureType.Cube;
                                            break;
                                        }
                                    case "minecraft:block/cube_column":
                                        {
                                            if (textures_jobj.TryGetValue("side", out var side) && TryGetName(side, out var sideName) &&
                                                textures_jobj.TryGetValue("end", out var end) && TryGetName(end, out var endName))
                                            {
                                                xp = sideName;
                                                xm = sideName;
                                                yp = endName;
                                                ym = endName;
                                                zp = sideName;
                                                zm = sideName;
                                            }
                                            else
                                                continue;
                                            type = BlockTextureType.CubeColumn;
                                            break;
                                        }
                                    case "minecraft:block/cube_bottom_top":
                                        {
                                            if (textures_jobj.TryGetValue("side", out var side) && TryGetName(side, out var sideName) &&
                                                textures_jobj.TryGetValue("top", out var top) && TryGetName(top, out var topName) &&
                                                textures_jobj.TryGetValue("bottom", out var bottom) && TryGetName(bottom, out var bottomName))
                                            {
                                                xp = sideName;
                                                xm = sideName;
                                                yp = topName;
                                                ym = bottomName;
                                                zp = sideName;
                                                zm = sideName;
                                            }
                                            else
                                                continue;
                                            type = BlockTextureType.CubeBottomTop;
                                            break;
                                        }
                                    case "block/block":
                                        {
                                            if (textures_jobj.TryGetValue("east", out var east) && TryGetName(east, out var eastName) &&
                                                textures_jobj.TryGetValue("west", out var west) && TryGetName(west, out var westName) &&
                                                textures_jobj.TryGetValue("up", out var up) && TryGetName(up, out var upName) &&
                                                textures_jobj.TryGetValue("down", out var down) && TryGetName(down, out var downName) &&
                                                textures_jobj.TryGetValue("south", out var south) && TryGetName(south, out var southName) &&
                                                textures_jobj.TryGetValue("north", out var north) && TryGetName(north, out var northName))
                                            {
                                                xp = eastName;
                                                xm = westName;
                                                yp = upName;
                                                ym = downName;
                                                zp = southName;
                                                zm = northName;
                                            }
                                            else
                                                continue;
                                            type = BlockTextureType.Block;
                                            break;
                                        }
                                    default:
                                        continue;
                                }

                                string id = "minecraft:" + Path.GetFileNameWithoutExtension(blockState);
                                if (!string.IsNullOrEmpty(variant_jpro.Name))
                                    id += $"[{variant_jpro.Name}]";
                                bool disable = false;
                                foreach (var black in blacklist)
                                {
                                    if (id.StartsWith(black))
                                    {
                                        disable = true;
                                        break;
                                    }
                                }
                                if (disable)
                                    continue;

                                Dictionary<Facing, Image<Rgba32>> images = new()
                                {
                                    { Facing.Xp, GetImage(xp) },
                                    { Facing.Xm, GetImage(xm) },
                                    { Facing.Yp, GetImage(yp) },
                                    { Facing.Ym, GetImage(ym) },
                                    { Facing.Zp, GetImage(zp) },
                                    { Facing.Zm, GetImage(zm) }
                                };

                                result.Add(id, new(id, type, images));
                            }
                        }
                    }
                }
            }

            return new(result);

            bool TryGetName(JToken jToken, [MaybeNullWhen(false)] out string result)
            {
                result = jToken.Value<string>()?.Split('/')[^1];
                return result is not null;
            }

            JObject GetModel(string name)
            {
                if (modelsCache.TryGetValue(name, out var model))
                    return model;
                else
                {
                    JObject jobj = JObject.Parse(File.ReadAllText(Path.Combine(resourcePack.Models.Block, name + ".json")));
                    modelsCache.Add(name, jobj);
                    return jobj;
                }
            }

            Image<Rgba32> GetImage(string name)
            {
                if (imagesCache.TryGetValue(name, out var image))
                    return image;
                else
                {
                    image = Image.Load<Rgba32>(File.ReadAllBytes(Path.Combine(resourcePack.Textures.Block, name + ".png")));
                    imagesCache.Add(name, image);
                    return image;
                }
            }
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

            foreach (var item in  _items)
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
