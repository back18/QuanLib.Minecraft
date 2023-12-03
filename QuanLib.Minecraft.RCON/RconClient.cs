using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.RCON
{
    public class RconClient : UnmanagedRunnable
    {
        public RconClient(ILogbuilder? logbuilder = null) : base(logbuilder)
        {
        }

        protected override void Run()
        {
            throw new NotImplementedException();
        }

        protected override void DisposeUnmanaged()
        {
            throw new NotImplementedException();
        }
    }
}
