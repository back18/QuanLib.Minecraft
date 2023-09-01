using QuanLib.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.DirectoryManagers
{
    public class SystemResourcesDirectory : DirectoryManager
    {
        public SystemResourcesDirectory(string directory) : base(directory)
        {
            Cursors = new(Combine("Cursors"));
            Fonts = new(Combine("Fonts"));
            Textures = new(Combine("Textures"));
        }

        public CursorsDirectory Cursors { get; }

        public FontsDirectory Fonts { get; }

        public TexturesDirectory Textures { get; }
    }
}
