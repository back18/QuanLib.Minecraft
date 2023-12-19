using QuanLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Directorys
{
    public class SavesDirectory : DirectoryBase
    {
        public SavesDirectory(string directory) : base(directory)
        {
        }

        public WorldDirectory? GetActiveWorldDirectory()
        {
            foreach (var directory in GetDirectorys())
            {
                WorldDirectory worldDirectory = new(directory);
                if (worldDirectory.IsLocked())
                    return worldDirectory;
            }

            return null;
        }
    }
}
