using QuanLib.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Directorys
{
    public class LogsDirectory : DirectoryBase
    {
        public LogsDirectory(string directory) : base(directory)
        {
            LatestFile = Combine("latest.log");
        }

        public string LatestFile { get; }
    }
}
