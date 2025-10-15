using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class ExecuteCommandBuilder : ICommandSyntax
    {
        public ICommandSyntax? Previous => null;

        public string GetSyntax()
        {
            return "execute";
        }

        public ExecuteCommandSyntax Execute()
        {
            return new ExecuteCommandSyntax(this);
        }
    }
}
