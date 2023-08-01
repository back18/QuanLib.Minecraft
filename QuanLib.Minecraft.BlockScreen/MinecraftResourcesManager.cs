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
            _BlockTextureManager = BlockTextureManager.LoadDirectory(Path.Combine(PathManager.MinecraftResources_Dir, "assets", "minecraft"));
        }
    }
}
