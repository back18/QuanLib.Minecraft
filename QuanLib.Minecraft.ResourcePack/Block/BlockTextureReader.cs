using Newtonsoft.Json.Linq;
using QuanLib.Core.Extensions;
using QuanLib.Game;
using QuanLib.Minecraft.ResourcePack.Block.BlockTextureMaps;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
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
        public static BlockTextureManager Load(ResourceEntryManager resources)
        {
            ArgumentNullException.ThrowIfNull(resources, nameof(resources));

            Dictionary<string, BlockTexture> result = new();
            ConcurrentDictionary<string, JObject> blockStates = GetBlockStates(resources);
            foreach (var blockState in blockStates)
            {
                if (!blockState.Value.TryGetValue("variants", out var variants))
                    continue;

                foreach (var variant in variants)
                {
                    if (variant is not JProperty variant_jpro ||
                        variant_jpro.Value is not JObject variant_jobj ||
                        !variant_jobj.TryGetValue("model", out var model) ||
                        !resources.TryGetModel(model.Value<string>(), out var model_jobj))
                        continue;

                    if (!model_jobj.TryGetValue("parent", out var parent) ||
                        !model_jobj.TryGetValue("textures", out var textures) ||
                        textures is not JObject textures_jobj)
                        continue;

                    string blockId = blockState.Key;
                    string state = variant_jpro.Name;
                    string blockIdState = blockId;
                    if (!string.IsNullOrEmpty(state))
                        blockIdState += $"[{state}]";

                    if (!TextureMap.TryGetTextureMap(parent.Value<string>(), state, out var textureMap) ||
                        !textureMap.TryParseJObject(textures_jobj, out var textureInfo))
                        continue;

                    if (!resources.TryGetTexture(textureInfo.Xp, out var xpTexture) ||
                        !resources.TryGetTexture(textureInfo.Xm, out var xmTexture) ||
                        !resources.TryGetTexture(textureInfo.Yp, out var ypTexture) ||
                        !resources.TryGetTexture(textureInfo.Ym, out var ymTexture) ||
                        !resources.TryGetTexture(textureInfo.Zp, out var zpTexture) ||
                        !resources.TryGetTexture(textureInfo.Zm, out var zmTexture))
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

                    result.Add(blockIdState, new(blockIdState, blockType, textureResult));
                }
            }

            return new(result);
        }

        private static ConcurrentDictionary<string, JObject> GetBlockStates(ResourceEntryManager resources)
        {
            ArgumentNullException.ThrowIfNull(resources, nameof(resources));

            ConcurrentDictionary<string, JObject> result = new();
            int total = 0;
            int count = 0;
            foreach (var resource in resources.Values)
            {
                total += resource.BlockStates.Count;
                Parallel.ForEach(resource.BlockStates.Values, blockState =>
                {
                    string extension = ".json";
                    if (!blockState.Name.EndsWith(extension))
                    {
                        Interlocked.Increment(ref count);
                        return;
                    }

                    string blockID = $"{resource.ModId}:{blockState.Name[..^extension.Length]}";
                    try
                    {
                        string text;
                        lock (resource.BlockStates)
                        {
                            text = blockState.ReadAllText();
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
