using QuanLib.IO;
using QuanLib.Minecraft.Mod.Forge.JarMetadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Forge
{
    public class ForgeJarInJarModFile : ForgeModFile
    {
        public ForgeJarInJarModFile(ForgeModFile owner, ZipPack ownerZip, MetadataInfo metadataInfo)
        {
            ArgumentNullException.ThrowIfNull(owner, nameof(owner));
            ArgumentNullException.ThrowIfNull(ownerZip, nameof(ownerZip));

            Owner = owner;
            MetadataInfo = metadataInfo;
            using ZipPack zipPack = ReadJarInJarModFile(ownerZip, metadataInfo);
            ModFileInfo = ReadFileInfo(zipPack);
            JarInJarModFiles = ReadJarInJarModFiles(zipPack).AsReadOnly();
        }

        public override ModSource ModSource => ModSource.JarInJar;

        public ForgeModFile Owner { get; }

        public MetadataInfo MetadataInfo { get; }

        public override string FilePath => MetadataInfo.Path;

        public override ForgeModFileInfo? ModFileInfo { get; }

        public override ReadOnlyCollection<ForgeJarInJarModFile> JarInJarModFiles { get; }

        public override string GetFileName()
        {
            return Owner.GetFileName() + "->" + base.GetFileName();
        }

        private static ZipPack ReadJarInJarModFile(ZipPack ownerZip, MetadataInfo metadataInfo)
        {
            ZipArchiveEntry entry = ownerZip[metadataInfo.Path];
            Stream stream = entry.Open();
            ZipPack zipPack = new(stream);
            return zipPack;
        }
    }
}
