using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public interface ICommandSyntax
    {
        public ICommandSyntax? Previous { get; }

        public string GetSyntax();
    }
}
