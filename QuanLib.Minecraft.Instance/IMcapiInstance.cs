using QuanLib.Minecraft.Instance.CommandSenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public interface IMcapiInstance : IMcapiCapable
    {
        public const string IDENTIFIER = "MCAPI";

        public McapiCommandSender McapiCommandSender { get; }
    }
}
