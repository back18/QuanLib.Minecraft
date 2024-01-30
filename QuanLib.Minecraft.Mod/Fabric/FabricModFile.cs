using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuanLib.Core.Extensions;
using QuanLib.IO.Zip;
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

            ZipItem zipItem = zipPack.GetFile("fabric.mod.json");
            using Stream stream = zipItem.OpenStream();
            string text = stream.ToUtf8Text();
            text = JsonFormatter.Format(text);

            FabricModInfo.DataModel modInfoModel = JsonConvert.DeserializeObject<FabricModInfo.DataModel>(text) ?? throw new FormatException();
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
