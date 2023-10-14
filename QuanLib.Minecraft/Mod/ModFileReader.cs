using QuanLib.Core.IO;
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
    public static class ModFileReader
    {
        private const string FORGE_MODINFO = "META-INF/mods.toml";
        private const string FABRIC_MODINFO = "fabric.mod.json";

        static ModFileReader()
        {
            _readers = new()
            {
                { ModLoaderType.Forge, new FabricModInfoReader() },
                { ModLoaderType.Fabric, new FabricModInfoReader() }
            };
        }

        private static readonly Dictionary<ModLoaderType, ModInfoReader> _readers;

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

            if (zipPack.TryGetValue(FORGE_MODINFO, out var forgeEntry))
            {
                return TryReadInfo(ModLoaderType.Forge, forgeEntry, out result);
            }
            else if (zipPack.TryGetValue(FABRIC_MODINFO, out var fabricEntry))
            {
                return TryReadInfo(ModLoaderType.Fabric, fabricEntry, out result);
            }
            else
            {
                goto fail;
            }

            fail:
            result = null;
            return false;
        }

        public static bool TryReadInfo(ModLoaderType type, ZipArchiveEntry entry, [MaybeNullWhen(false)] out ModInfo result)
        {
            using Stream stream = entry.Open();
            return TryReadInfo(type, stream, out result);
        }

        public static bool TryReadInfo(ModLoaderType type, Stream stream, [MaybeNullWhen(false)] out ModInfo result)
        {
            if (_readers.TryGetValue(type, out var reader) && reader.TryRead(stream, out result))
            {
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}
