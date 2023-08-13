using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Config
{
    public static class ConfigManager
    {
        public static MinecraftConfig MinecraftConfig
        {
            get
            {
                if (_MinecraftConfig is null)
                    throw new InvalidOperationException();
                return _MinecraftConfig;
            }
        }
        private static MinecraftConfig? _MinecraftConfig;

        public static SystemConfig SystemConfig
        {
            get
            {
                if (_SystemConfig is null)
                    throw new InvalidOperationException();
                return _SystemConfig;
            }
        }
        private static SystemConfig? _SystemConfig;


        public static ScreenConfig ScreenConfig
        {
            get
            {
                if (_ScreenConfig is null)
                    throw new InvalidOperationException();
                return _ScreenConfig;
            }
        }
        private static ScreenConfig? _ScreenConfig;

        public static IReadOnlyDictionary<string, string> Registry
        {
            get
            {
                if (_Registry is null)
                    throw new InvalidOperationException();
                return _Registry;
            }
        }
        private static Dictionary<string, string>? _Registry;

        public static void LoadAll()
        {
            _MinecraftConfig = MinecraftConfig.Load(PathManager.Configs_Minecraft_File);
            _SystemConfig = SystemConfig.Load(PathManager.Configs_System_File);
            _ScreenConfig = ScreenConfig.Load(PathManager.Configs_Screen_File);

            _Registry = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(PathManager.Configs_Registry_File)) ?? throw new FormatException();
        }
    }
}
