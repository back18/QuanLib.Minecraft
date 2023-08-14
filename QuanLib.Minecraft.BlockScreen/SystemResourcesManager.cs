using QuanLib.BDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public static class SystemResourcesManager
    {
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
            _CursorManager = CursorManager.Load(MCOS.MainDirectory.SystemResources.Cursors.Directory);
        }
    }
}
