using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Selectors
{
    public abstract class Selector
    {
        public override string ToString()
        {
            return Target.CommandExecutor.ToCommandArgument();
        }
    }
}
