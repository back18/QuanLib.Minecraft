using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuanLib.Core.Extensions;
using QuanLib.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Fabric
{
    public abstract class FabricModFile
    {
        public abstract ModSource ModSource { get; }

        public abstract string FilePath { get; }

        public abstract FabricModInfo ModInfo { get; }

        public abstract ReadOnlyCollection<FabricJarInJarModFile> JarInJarModFiles { get; }

        public virtual FabricMod[] GetAllMod()
        {
            List<FabricMod> result = [new FabricMod(this, ModInfo)];
            foreach (FabricJarInJarModFile jarInJarModFile in JarInJarModFiles)
                result.AddRange(jarInJarModFile.GetAllMod());

            return result.ToArray();
        }

        public virtual string GetFileName()
        {
            return Path.GetFileName(FilePath);
        }

        public override string ToString()
        {
            return $"{ModSource} - {GetFileName()} - [{string.Join(", ", GetAllMod().Select(s => s.ModInfo.ModId))}]";
        }

        protected virtual FabricModInfo ReadModInfo(ZipPack zipPack)
        {
            ArgumentNullException.ThrowIfNull(zipPack, nameof(zipPack));

            ZipArchiveEntry entry = zipPack["fabric.mod.json"];
            using Stream stream = entry.Open();
            string text = stream.ToUtf8Text();
            text = JsonFormatter.Format(text);

            FabricModInfo.DataModel modInfoModel = FabricModInfo.DataModel.CreateDefault();
            JsonConvert.PopulateObject(text, modInfoModel);
            return FabricModInfo.FromDataModel(modInfoModel);
        }

        protected virtual FabricJarInJarModFile[] ReadJarInJarModFiles(ZipPack zipPack, FabricModInfo modInfo)
        {
            ArgumentNullException.ThrowIfNull(zipPack, nameof(zipPack));
            ArgumentNullException.ThrowIfNull(modInfo, nameof(modInfo));

            List<FabricJarInJarModFile> result = [];
            foreach (JarInfo jarInfo in modInfo.Jars)
            {
                result.Add(new(this, zipPack, jarInfo.File));
            }

            return result.ToArray();
        }
    }
}
