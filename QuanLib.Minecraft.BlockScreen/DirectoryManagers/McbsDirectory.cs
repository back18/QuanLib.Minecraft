using QuanLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.DirectoryManagers
{
    public class McbsDirectory : DirectoryManager
    {
        public McbsDirectory(string directory) : base(directory)
        {
            Applications = new(Combine("Applications"));
            Configs = new(Combine("Configs"));
            FFmpeg = new(Combine("FFmpeg"));
            MinecraftResources = new(Combine("MinecraftResources"));
            SystemResources = new(Combine("SystemResources"));
        }

        public ApplicationsDirectory Applications { get; }

        public ConfigsDirectory Configs { get; }

        public FFmpegDirectory FFmpeg { get; }

        public MinecraftResourcesDirectory MinecraftResources { get; }

        public SystemResourcesDirectory SystemResources { get; }
    }
}
