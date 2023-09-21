using static QuanLib.Minecraft.BlockScreen.Config.ConfigManager;
using log4net.Core;
using QuanLib.Minecraft.BlockScreen.Logging;
using QuanLib.Minecraft.ResourcePack;
using QuanLib.Minecraft.ResourcePack.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.ResourcePack.Language;

namespace QuanLib.Minecraft.BlockScreen
{
    public static class MinecraftResourcesManager
    {
        private static readonly LogImpl LOGGER = LogUtil.MainLogger;

        public static void LoadAll()
        {
            string[] paths = new string[MinecraftConfig.ResourcePackList.Count];
            for (int i = 0; i < paths.Length; i++)
                paths[i] = MCOS.MainDirectory.MinecraftResources.ResourcePacks.Combine(MinecraftConfig.ResourcePackList[i]);
            ResourceEntryManager resources = ResourcePackReader.Load(paths);

            LOGGER.Info($"开始加载Minecraft方块纹理文件");
            LOGGER.Info($"共计{MinecraftConfig.ResourcePackList.Count}个资源包，资源包列表：\n{string.Join('\n', MinecraftConfig.ResourcePackList)}");
            LOGGER.Info($"已禁用{MinecraftConfig.BlockTextureBlacklist.Count}个方块纹理，方块纹理黑名单列表：\n{string.Join('\n', MinecraftConfig.BlockTextureBlacklist)}");
            BlockTextureManager.LoadInstance(resources, MinecraftConfig.BlockTextureBlacklist);
            LOGGER.Info($"Minecraft方块纹理文件加载完成，数量:{BlockTextureManager.Instance.Count}");

            string? minecraftLanguageFilePath = MCOS.MainDirectory.MinecraftResources.Languages.Combine(MinecraftConfig.Language + ".json");
            if (!File.Exists(minecraftLanguageFilePath))
                minecraftLanguageFilePath = null;

            var ins = LanguageManager.LoadInstance(resources, MinecraftConfig.Language, minecraftLanguageFilePath);

            resources.Dispose();
        }
    }
}
