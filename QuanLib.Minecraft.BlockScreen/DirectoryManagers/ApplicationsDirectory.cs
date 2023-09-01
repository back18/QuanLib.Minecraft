using QuanLib.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.DirectoryManagers
{
    public class ApplicationsDirectory : DirectoryManager
    {
        public ApplicationsDirectory(string directory) : base(directory)
        {
        }

        public string GetApplicationDirectory(string id)
        {
            return Combine(id);
        }
    }
}
