using QuanLib.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.DirectoryManagers
{
    public class SavesDirectory : DirectoryManager
    {
        public SavesDirectory(string directory) : base(directory)
        {
            Interactions = new(Combine("Interactions"));
            ScreenSaves = Combine("ScreenSaves.json");
        }

        public InteractionsDirectory Interactions { get; }

        public string ScreenSaves { get; }
    }
}
