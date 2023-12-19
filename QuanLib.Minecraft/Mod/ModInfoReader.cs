using QuanLib.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod
{
    public static class ModInfoReader
    {
        static ModInfoReader()
        {
            _parsers = new()
            {
                { ModLoaderType.Forge, new ForgeModInfoParser() },
                { ModLoaderType.Fabric, new FabricModInfoParser() }
            };
        }

        private static readonly Dictionary<ModLoaderType, ModInfoParser> _parsers;

        public static bool TryReadFile(string path, [MaybeNullWhen(false)] out ModInfo result)
        {
            if (string.IsNullOrEmpty(path))
                goto fail;

            try
            {
                using ZipPack zipPack = new(path);
                return TryReadFile(zipPack, out result);
            }
            catch
            {
                goto fail;
            }

            fail:
            result = null;
            return false;
        }

        public static bool TryReadFile(Stream stream, [MaybeNullWhen(false)] out ModInfo result)
        {
            if (stream is null)
                goto fail;

            try
            {
                using ZipPack zipPack = new(stream);
                return TryReadFile(zipPack, out result);
            }
            catch
            {
                goto fail;
            }

            fail:
            result = null;
            return false;

        }

        public static bool TryReadFile(ZipPack zipPack, [MaybeNullWhen(false)] out ModInfo result)
        {
            if (zipPack is null)
                goto fail;

            foreach (var parse in _parsers.Values)
            {
                if (zipPack.ContainsKey(parse.ModInfoPath))
                    return parse.TryParse(zipPack, out result);
            }

            goto fail;

            fail:
            result = null;
            return false;
        }
    }
}
