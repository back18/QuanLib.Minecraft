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
            if (string.IsNullOrWhiteSpace(javaPath))
                throw new ArgumentException($"“{nameof(javaPath)}”不能为 null 或空白。", nameof(javaPath));

            JavaPath = javaPath;
        }

        public string JavaPath { get; }

        public abstract string GetArguments();
    }
}
