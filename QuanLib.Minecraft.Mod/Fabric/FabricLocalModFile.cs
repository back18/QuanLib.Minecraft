using QuanLib.IO.Zip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Fabric
{
    public class FabricLocalModFile : FabricModFile
    {
        public FabricLocalModFile(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            FilePath = filePath;
            using ZipPack zipPack = new(filePath);
            ModInfo = ReadModInfo(zipPack);
            JarInJarModFiles = ReadJarInJarModFiles(zipPack, ModInfo).AsReadOnly();
        }

        public override ModSource ModSource => ModSource.Local;

        public override string FilePath { get; }

        public override FabricModInfo ModInfo { get; }

        public override ReadOnlyCollection<FabricJarInJarModFile> JarInJarModFiles { get; }
    }
}
