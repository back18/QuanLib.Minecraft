using QuanLib.Minecraft.API;
using QuanLib.Minecraft.Instance.CommandSenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public interface IMcapiInstance
    {
        public const string INSTANCE_KEY = "MCAPI";

        public ushort McapiPort { get; }

        public string McapiPassword { get; }

        public McapiClient McapiClient { get; }

        public McapiCommandSender McapiCommandSender { get; }
    }
}
