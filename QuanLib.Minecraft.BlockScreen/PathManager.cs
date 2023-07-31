using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public static class PathManager
    {
        static PathManager()
        {
            Main_Dir = "MCBS";
            MinecraftConfig_File = Path.Combine(Main_Dir, "Minecraft.json");
            SystemConfig_File = Path.Combine(Main_Dir, "System.json");
            ScreenConfig_File = Path.Combine(Main_Dir, "Screen.json");
            Applications_Dir = Path.Combine(Main_Dir, "Applications");
            FFmpeg_Dir = Path.Combine(Main_Dir, "FFmpeg");
            MinecraftResources_Dir = Path.Combine(Main_Dir, "MinecraftResources");
            SystemResources_Dir = Path.Combine(Main_Dir, "SystemResources");
            SystemResources_Fonts_Dir = Path.Combine(SystemResources_Dir, "Fonts");
            SystemResources_Textures_Dir = Path.Combine(SystemResources_Dir, "Textures");
            SystemResources_Textures_Control_Dir = Path.Combine(SystemResources_Textures_Dir, "Control");
            SystemResources_Textures_Cursor_Dir = Path.Combine(SystemResources_Textures_Dir, "Cursor");
            SystemResources_Textures_Icon_Dir = Path.Combine(SystemResources_Textures_Dir, "Icon");
        }

        public static string Main_Dir { get; }

        public static string MinecraftConfig_File { get; }

        public static string SystemConfig_File { get; }

        public static string ScreenConfig_File { get; }

        public static string Applications_Dir { get; }

        public static string FFmpeg_Dir { get; }

        public static string MinecraftResources_Dir { get; }

        public static string SystemResources_Dir { get; }

        public static string SystemResources_Fonts_Dir { get; }

        public static string SystemResources_Textures_Dir { get; }

        public static string SystemResources_Textures_Control_Dir { get; }

        public static string SystemResources_Textures_Cursor_Dir { get; }

        public static string SystemResources_Textures_Icon_Dir { get; }

        public static string GetApplicationDir(string id)
        {
            return Path.Combine(Applications_Dir, id);
        }
    }
}
