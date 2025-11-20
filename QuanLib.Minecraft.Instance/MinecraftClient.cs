using QuanLib.Core;
using QuanLib.Minecraft.PathManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public abstract class MinecraftClient(string clientPath, ILoggerProvider? loggerProvider = null) : MinecraftInstance(clientPath, loggerProvider)
    {
        public override bool IsClient => true;

        public override MinecraftPathManager MinecraftPathManager => ClientPathManager;

        public virtual ClientPathManager ClientPathManager { get; } = new(clientPath);
    }
}
