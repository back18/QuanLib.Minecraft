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
    public class FabricJarInJarModFile : FabricModFile
    {
        public FabricJarInJarModFile(FabricModFile owner, ZipPack ownerZip, string filePath)
        {
            ArgumentNullException.ThrowIfNull(owner, nameof(owner));
            ArgumentNullException.ThrowIfNull(ownerZip, nameof(ownerZip));
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            Owner = owner;
            FilePath = filePath;
            using ZipPack zipPack = ReadJarInJarModFile(ownerZip, filePath);
            ModInfo = ReadModInfo(zipPack);
            JarInJarModFiles = ReadJarInJarModFiles(zipPack, ModInfo).AsReadOnly();
        }

        public override ModSource ModSource => ModSource.JarInJar;

        public FabricModFile Owner { get; }

        public override string FilePath { get; }

        public override FabricModInfo ModInfo { get; }

        public override ReadOnlyCollection<FabricJarInJarModFile> JarInJarModFiles { get; }

        public override string GetFileName()
        {
            return Owner.GetFileName() + "->" + base.GetFileName();
        }

        private static ZipPack ReadJarInJarModFile(ZipPack ownerZip, string filePath)
        {
            ZipItem zipItem = ownerZip.GetFile(filePath);
            Stream stream = zipItem.OpenStream();
            ZipPack zipPack = new(stream);
            return zipPack;
        }
    }
}
