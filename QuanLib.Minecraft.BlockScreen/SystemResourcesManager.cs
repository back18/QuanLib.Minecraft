using log4net.Core;
using QuanLib.BDF;
using QuanLib.Minecraft.BlockScreen.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public static class SystemResourcesManager
    {
        private static readonly LogImpl LOGGER = LogUtil.MainLogger;

        public static BdfFont DefaultFont
        {
            get
            {
                if (_DefaultFont is null)
                    throw new InvalidOperationException();
                return _DefaultFont;
            }
        }
        private static BdfFont? _DefaultFont;

        public static CursorManager CursorManager
        {
            get
            {
                if (_CursorManager is null)
                    throw new InvalidOperationException();
                return _CursorManager;
            }
        }
        private static CursorManager? _CursorManager;

        public static void LoadAll()
        {
            _DefaultFont = BdfFont.Load(Path.Combine(MCOS.MainDirectory.SystemResources.Fonts.DefaultFont));
            LOGGER.Info($"默认字体文件加载完成，高度:{DefaultFont.Height} 半角宽度:{DefaultFont.HalfWidth} 全角宽度:{DefaultFont.FullWidth} 字符数量:{DefaultFont.Count}");

            _CursorManager = CursorManager.Load(MCOS.MainDirectory.SystemResources.Cursors.Directory);
            LOGGER.Info($"光标文件加载完成");
        }
    }
}
