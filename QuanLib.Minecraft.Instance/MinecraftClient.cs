using QuanLib.Core;
using QuanLib.Minecraft.PathManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public abstract class MinecraftClient : MinecraftInstance
    {
        protected MinecraftClient(string clientPath, ILoggerGetter? loggerGetter = null) : base(clientPath, loggerGetter)
        {
            ClientPathManager = new(clientPath);
        }

        public override InstanceType InstanceType => InstanceType.Client;

        public override MinecraftPathManager MinecraftPathManager => ClientPathManager is null ? base.MinecraftPathManager : ClientPathManager;

        public virtual ClientPathManager ClientPathManager { get; }
    }
}
