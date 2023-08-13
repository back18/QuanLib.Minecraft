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
            Configs_Dir = Path.Combine(Main_Dir, "Configs");
            Configs_Minecraft_File = Path.Combine(Configs_Dir, "Minecraft.json");
            Configs_System_File = Path.Combine(Configs_Dir, "System.json");
            Configs_Screen_File = Path.Combine(Configs_Dir, "Screen.json");
            Configs_Registry_File = Path.Combine(Configs_Dir, "Registry.json");
            Applications_Dir = Path.Combine(Main_Dir, "Applications");
            FFmpeg_Dir = Path.Combine(Main_Dir, "FFmpeg");
            MinecraftResources_Dir = Path.Combine(Main_Dir, "MinecraftResources");
            SystemResources_Dir = Path.Combine(Main_Dir, "SystemResources");
            SystemResources_Fonts_Dir = Path.Combine(SystemResources_Dir, "Fonts");
            SystemResources_Cursors_Dir = Path.Combine(SystemResources_Dir, "Cursors");
            SystemResources_Textures_Dir = Path.Combine(SystemResources_Dir, "Textures");
            SystemResources_Textures_Control_Dir = Path.Combine(SystemResources_Textures_Dir, "Control");
            SystemResources_Textures_Icon_Dir = Path.Combine(SystemResources_Textures_Dir, "Icon");
        }

        public static string Main_Dir { get; }

        public static string Configs_Dir { get; }

        public static string Configs_Minecraft_File { get; }

        public static string Configs_System_File { get; }

        public static string Configs_Screen_File { get; }


        public static string Configs_Registry_File { get; }

        public static string Applications_Dir { get; }

        public static string FFmpeg_Dir { get; }

        public static string MinecraftResources_Dir { get; }

        public static string SystemResources_Dir { get; }

        public static string SystemResources_Fonts_Dir { get; }

        public static string SystemResources_Cursors_Dir { get; }

        public static string SystemResources_Textures_Dir { get; }

        public static string SystemResources_Textures_Control_Dir { get; }

        public static string SystemResources_Textures_Icon_Dir { get; }

        public static string GetApplicationDir(string id)
        {
            return Path.Combine(Applications_Dir, id);
        }
    }
}
