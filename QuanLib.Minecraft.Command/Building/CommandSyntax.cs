using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public abstract class CommandSyntax(ICommandSyntax? previous) : ICommandSyntax
    {
        private string? _syntax;

        public ICommandSyntax? Previous { get; protected set; } = previous;

        protected virtual void SetSyntax(string syntax)
        {
            _syntax = syntax;
        }

        public virtual string GetSyntax()
        {
            if (_syntax is null)
                throw new InvalidOperationException("语法未设置");
            return _syntax;
        }
    }
}
