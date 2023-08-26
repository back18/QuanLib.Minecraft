using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack.Block.BlockTextureMaps
{
    public abstract class TextureMap
    {
        public abstract string Name { get; }

        public abstract BlockType Type { get; }

        public abstract string Xp { get; }

        public abstract string Xm { get; }

        public abstract string Yp { get; }

        public abstract string Ym { get; }

        public abstract string Zp { get; }

        public abstract string Zm { get; }

        public bool TryParseJObject(JObject jobject, [MaybeNullWhen(false)] out TextureInfo result)
        {
            if (jobject is not null &&
                jobject.TryGetValue(Xp, out var xp) && TryGetValue(xp, out var xp_value) &&
                jobject.TryGetValue(Xm, out var xm) && TryGetValue(xm, out var xm_value) &&
                jobject.TryGetValue(Yp, out var yp) && TryGetValue(yp, out var yp_value) &&
                jobject.TryGetValue(Ym, out var ym) && TryGetValue(ym, out var ym_value) &&
                jobject.TryGetValue(Zp, out var zp) && TryGetValue(zp, out var zp_value) &&
                jobject.TryGetValue(Zm, out var zm) && TryGetValue(zm, out var zm_value))
            {
                result = new(Type, xp_value, xm_value, yp_value, ym_value, zp_value, zm_value);
                return true;
            }
            else
            {
                result = null;
                return false;
            }

            bool TryGetValue(JToken jtoken, [MaybeNullWhen(false)] out string result)
            {
                result = jtoken.Value<string>();
                return result is not null;
            }
        }

        public static bool TryGetTextureMap(string? name, string blockState, [MaybeNullWhen(false)] out TextureMap result)
        {
            switch (name)
            {
                case "minecraft:block/block":
                    result = new BlockTextureMap();
                    return true;
                case "minecraft:block/cube":
                    result = new CubeTextureMap();
                    return true;
                case "minecraft:block/cube_all":
                    result = new CubeAllTextureMap();
                    return true;
                case "minecraft:block/cube_column":
                    result = new CubeColumnTextureMap(blockState);
                    return true;
                case "minecraft:block/cube_bottom_top":
                    result = new CubeBottomTopTextureMap();
                    return true;
                case "minecraft:block/orientable":
                    result = new OrientableTextureMap(blockState);
                    return true;
                default:
                    result = null;
                    return false;
            }
        }
    }
}
