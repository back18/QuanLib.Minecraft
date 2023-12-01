using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public abstract class ServerLaunchArguments
    {
        protected ServerLaunchArguments(string javaPath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(javaPath, nameof(javaPath));

            JavaPath = javaPath;
        }

        public string JavaPath { get; }

        public abstract string GetArguments();
    }
}
