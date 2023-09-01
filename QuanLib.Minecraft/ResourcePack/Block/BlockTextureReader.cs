using Newtonsoft.Json.Linq;
using QuanLib.Core.Extension;
using QuanLib.Minecraft.ResourcePack.Block.BlockTextureMaps;
using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack.Block
{
    public static class BlockTextureReader
    {
        public static BlockTextureManager Load(ResourceEntryManager entrys, IEnumerable<BlockState> blacklist)
        {
            if (entrys is null)
                throw new ArgumentNullException(nameof(entrys));

            Dictionary<string, BlockTexture> result = new();
            ConcurrentDictionary<string, JObject> blockStates = GetBlockStates(entrys);
            foreach (var blockState in blockStates)
            {
                if (!blockState.Value.TryGetValue("variants", out var variants))
                    continue;

                foreach (var variant in variants)
                {
                    if (variant is not JProperty variant_jpro ||
                        variant_jpro.Value is not JObject variant_jobj ||
                        !variant_jobj.TryGetValue("model", out var model) ||
                        !entrys.TryGetModel(model.Value<string>(), out var model_jobj))
                        continue;

                    if (!model_jobj.TryGetValue("parent", out var parent) ||
                        !model_jobj.TryGetValue("textures", out var textures) ||
                        textures is not JObject textures_jobj)
                        continue;

                    string blockID = blockState.Key;
                    string state = variant_jpro.Name;
                    Dictionary<string, string>? states = new();
                    string key = blockID;
                    if (!string.IsNullOrEmpty(state))
                    {
                        if (!MinecraftUtil.TryParseBlockState(state, out states))
                            states = new();
                        key += $"[{state}]";
                    }

                    bool disable = false;
                    foreach (var black in blacklist)
                    {
                        if (blockID != black.BlockID)
                            continue;

                        foreach (var item in black.States)
                        {
                            if (!states.TryGetValue(item.Key, out var value) || value != item.Value)
                                continue;
                        }

                        disable = true;
                        break;
                    }
                    if (disable)
                        continue;

                    if (!TextureMap.TryGetTextureMap(parent.Value<string>(), state, out var textureMap) ||
                        !textureMap.TryParseJObject(textures_jobj, out var textureInfo))
                        continue;

                    if (!entrys.TryGetTexture(textureInfo.Xp, out var xpTexture) ||
                        !entrys.TryGetTexture(textureInfo.Xm, out var xmTexture) ||
                        !entrys.TryGetTexture(textureInfo.Yp, out var ypTexture) ||
                        !entrys.TryGetTexture(textureInfo.Ym, out var ymTexture) ||
                        !entrys.TryGetTexture(textureInfo.Zp, out var zpTexture) ||
                        !entrys.TryGetTexture(textureInfo.Zm, out var zmTexture))
                        continue;

                    BlockType blockType = textureInfo.Type;

                    Dictionary<Facing, Image<Rgba32>> textureResult = new()
                    {
                        { Facing.Xp, xpTexture },
                        { Facing.Xm, xmTexture },
                        { Facing.Yp, ypTexture },
                        { Facing.Ym, ymTexture },
                        { Facing.Zp, zpTexture },
                        { Facing.Zm, zmTexture }
                    };

                    result.Add(key, new(key, blockType, textureResult));
                }
            }

            return new(result);
        }

        private static ConcurrentDictionary<string, JObject> GetBlockStates(ResourceEntryManager entrys)
        {
            if (entrys is null)
                throw new ArgumentNullException(nameof(entrys));

            ConcurrentDictionary<string, JObject> result = new();
            int total = 0;
            int count = 0;
            foreach (var entry in entrys.Values)
            {
                total += entry.BlockStates.Count;
                Parallel.ForEach(entry.BlockStates.Values, blockState =>
                {
                    string extension = ".json";
                    if (!blockState.Name.EndsWith(extension))
                    {
                        Interlocked.Increment(ref count);
                        return;
                    }

                    string blockID = $"{entry.ModID}:{blockState.Name[..^extension.Length]}";
                    try
                    {
                        string text;
                        lock (entry.BlockStates)
                        {
                            using Stream stream = blockState.Open();
                            text = stream.ToUtf8Text();
                        }
                        result.TryAdd(blockID, JObject.Parse(text));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"无法解析方块状态json文件“{blockID}”\n{ex.GetType().Name}: {ex.Message}");
                    }

                    Interlocked.Increment(ref count);
                });
            }

            while (count < total)
                Thread.Sleep(10);

            return result;
        }
    }
}
