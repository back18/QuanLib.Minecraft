﻿using QuanLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Directorys
{
    public class WorldDirectory : DirectoryBase
    {
        public WorldDirectory(string directory) : base(directory)
        {
            SessionLockFile = Combine("session.lock");
        }

        public string SessionLockFile { get; }

        public bool IsLocked()
        {
            if (!File.Exists(SessionLockFile))
                return false;

            try
            {
                using FileStream fileStream = new(SessionLockFile, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write | FileShare.Delete);
                fileStream.Seek(0, SeekOrigin.Begin);
                fileStream.ReadByte();
                return false;
            }
            catch
            {
                return true;
            }
        }
    }
}
