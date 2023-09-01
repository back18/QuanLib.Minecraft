using QuanLib.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.DirectoryManagers
{
    public class TexturesDirectory : DirectoryManager
    {
        public TexturesDirectory(string directory) : base(directory)
        {
            Control = Combine("Control");
            Icon = Combine("Icon");
        }

        public string Control { get; }

        public string Icon { get; }
    }
}
