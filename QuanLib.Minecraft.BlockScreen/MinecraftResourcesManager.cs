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

namespace QuanLib.Minecraft.BlockScreen
{
    public static class MinecraftResourcesManager
    {
        private static readonly LogImpl LOGGER = LogUtil.MainLogger;

        public static BlockTextureManager BlockTextureManager
        {
            get
            {
                if (_BlockTextureManager is null)
                    throw new InvalidOperationException();
                return _BlockTextureManager;
            }
        }
        private static BlockTextureManager? _BlockTextureManager;

        public static void LoadAll()
        {
            LOGGER.Info($"开始加载Minecraft方块纹理文件");
            LOGGER.Info($"共计{MinecraftConfig.ResourcePackList.Count}个资源包，资源包列表：\n{string.Join('\n', MinecraftConfig.ResourcePackList)}");
            LOGGER.Info($"已禁用{MinecraftConfig.BlockTextureBlacklist.Count}个方块纹理，方块纹理黑名单列表：\n{string.Join('\n', MinecraftConfig.BlockTextureBlacklist)}");

            string[] paths = new string[MinecraftConfig.ResourcePackList.Count];
            for (int i = 0; i < paths.Length; i++)
                paths[i] = MCOS.MainDirectory.MinecraftResources.ResourcePacks.Combine(MinecraftConfig.ResourcePackList[i]);

            ResourceEntryManager entrys = ResourcePackReader.Load(paths);
            _BlockTextureManager = BlockTextureReader.Load(entrys, MinecraftConfig.BlockTextureBlacklist);
            entrys.Dispose();

            LOGGER.Info($"Minecraft方块纹理文件加载完成，数量:{BlockTextureManager.Count}");
        }
    }
}
