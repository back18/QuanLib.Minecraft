using QuanLib.IO.Zip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Forge
{
    public class ForgeLocalModFile : ForgeModFile
    {
        public ForgeLocalModFile(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            FilePath = filePath;
            using ZipPack zipPack = new(filePath);
            ModFileInfo = ReadFileInfo(zipPack);
            JarInJarModFiles = ReadJarInJarModFiles(zipPack).AsReadOnly();
        }

        public override ModSource ModSource => ModSource.Local;

        public override string FilePath { get; }

        public override ForgeModFileInfo? ModFileInfo { get; }

        public override ReadOnlyCollection<ForgeJarInJarModFile> JarInJarModFiles { get; }
    }
}
