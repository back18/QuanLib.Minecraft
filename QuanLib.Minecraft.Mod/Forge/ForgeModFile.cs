using Nett;
using Newtonsoft.Json.Linq;
using QuanLib.Core.Extensions;
using QuanLib.IO;
using QuanLib.Minecraft.Mod.Forge.JarMetadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Forge
{
    public abstract class ForgeModFile
    {
        public abstract ModSource ModSource { get; }

        public abstract string FilePath { get; }

        public abstract ForgeModFileInfo? ModFileInfo { get; }

        public abstract ReadOnlyCollection<ForgeJarInJarModFile> JarInJarModFiles { get; }

        public virtual ForgeMod[] GetAllMod()
        {
            List<ForgeMod> result = [];
            if (ModFileInfo is not null)
            {
                foreach (ForgeModInfo modInfo in ModFileInfo.ModInfos)
                    result.Add(new(this, modInfo));
            }

            foreach (ForgeJarInJarModFile jarInJarModFile in JarInJarModFiles)
                result.AddRange(jarInJarModFile.GetAllMod());

            return result.ToArray();
        }

        public virtual string GetFileName()
        {
            return Path.GetFileName(FilePath);
        }

        protected virtual ForgeModFileInfo? ReadFileInfo(ZipPack zipPack)
        {
            ArgumentNullException.ThrowIfNull(zipPack, nameof(zipPack));

            if (!zipPack.TryGetValue("META-INF/mods.toml", out var entry))
                return null;

            using Stream stream = entry.Open();
            TomlTable tomlTable = Toml.ReadStream(stream);
            ForgeModFileInfo.DataModel fileInfoModel = tomlTable.Get<ForgeModFileInfo.DataModel>();

            foreach (var modInfoModel in fileInfoModel.mods)
            {
                ReadDependencies(tomlTable, modInfoModel);
                ReadFeatures(tomlTable, modInfoModel);
                ReadModProperties(tomlTable, modInfoModel);
                ReadVersion(zipPack, modInfoModel);
            }

            return ForgeModFileInfo.FromDataModel(fileInfoModel);
        }

        protected virtual ForgeJarInJarModFile[] ReadJarInJarModFiles(ZipPack zipPack)
        {
            ArgumentNullException.ThrowIfNull(zipPack, nameof(zipPack));

            if (!zipPack.TryGetValue("META-INF/jarjar/metadata.json", out var entry))
                return [];

            using Stream stream = entry.Open();
            string text = stream.ToUtf8Text();
            JObject jObject1 = JObject.Parse(text);
            if (!jObject1.TryGetValue("jars", out var jars) || jars is not JArray jArray)
                return [];

            List<ForgeJarInJarModFile> result = [];
            foreach (JToken jToken in jArray)
            {
                MetadataInfo metadataInfo = jToken.ToObject<MetadataInfo>();
                result.Add(new(this, zipPack, metadataInfo));
            }

            return result.ToArray();
        }

        public override string ToString()
        {
            return $"{ModSource} - {GetFileName()} - [{string.Join(", ", GetAllMod().Select(s => s.ModInfo.ModId))}]";
        }

        protected static void ReadVersion(ZipPack zipPack, ForgeModInfo.DataModel modInfoModel)
        {
            if (modInfoModel.version == "${file.jarVersion}" && zipPack.TryGetValue("META-INF/MANIFEST.MF", out var manifestEntry))
            {
                using Stream stream = manifestEntry.Open();
                string text = stream.ToUtf8Text();
                Dictionary<string, string> imanifest = ManifestParser.Parse(text);
                if (imanifest.TryGetValue("Implementation-Version", out var version))
                    modInfoModel.version = version;
            }
        }

        protected static void ReadDependencies(TomlTable tomlTable, ForgeModInfo.DataModel modInfoModel)
        {
            var dependencies = tomlTable.GetValueAs<TomlTableArray>("dependencies")?.Items.FirstOrDefault()?.GetValueAs<TomlTableArray>(modInfoModel.modId);
            if (dependencies is not null)
            {
                foreach (var item in dependencies.Items)
                {
                    ForgeModVersion.DataModel ModVersionModel = item.Get<ForgeModVersion.DataModel>();
                    modInfoModel.dependencies.Add(ModVersionModel);
                }
            }
        }

        protected static void ReadFeatures(TomlTable tomlTable, ForgeModInfo.DataModel modInfoModel)
        {
            var features = tomlTable.GetValueAs<TomlTableArray>("features")?.Items.FirstOrDefault()?.GetValueAs<TomlTableArray>(modInfoModel.modId);
            if (features is not null)
            {
                //TODO
            }
        }

        protected static void ReadModProperties(TomlTable tomlTable, ForgeModInfo.DataModel modInfoModel)
        {
            var modproperties = tomlTable.GetValueAs<TomlTableArray>("modproperties")?.Items.FirstOrDefault()?.GetValueAs<TomlTableArray>(modInfoModel.modId);
            if (modproperties is not null)
            {
                //TODO
            }
        }
    }
}
