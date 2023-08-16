using log4net.Core;
using QuanLib.Minecraft.BlockScreen.Logging;
using QuanLib.Minecraft.Files;
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
            _BlockTextureManager = BlockTextureManager.LoadDirectory(Path.Combine(MCOS.MainDirectory.MinecraftResources.Directory, "assets", "minecraft"));
            LOGGER.Info($"Minecraft方块贴图文件加载完成，数量:{BlockTextureManager.Count}");
        }
    }
}
