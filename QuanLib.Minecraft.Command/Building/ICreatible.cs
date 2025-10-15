using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public interface ICreatible<TSelf> where TSelf : ICreatible<TSelf>
    {
        public static abstract TSelf Create(ICommandSyntax? previous);
    }
}
