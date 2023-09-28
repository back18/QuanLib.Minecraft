using log4net.Core;
using QuanLib.Minecraft.Directorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public abstract class MinecraftClient : MinecraftInstance
    {
        protected MinecraftClient(string clientPath, Func<Type, LogImpl> logger) : base(clientPath, logger)
        {
            ClientDirectory = new(clientPath);
        }

        public override InstanceType InstanceType => InstanceType.Client;

        public override MinecraftDirectory MinecraftDirectory => ClientDirectory is null ? base.MinecraftDirectory : ClientDirectory;

        public virtual MinecraftClientDirectory ClientDirectory { get; }
    }
}
