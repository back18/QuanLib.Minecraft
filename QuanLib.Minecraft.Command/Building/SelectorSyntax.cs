using QuanLib.Minecraft.Command.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class SelectorSyntax<TNext>(ICommandSyntax? previous) : CommandSyntax(previous) where TNext : ICreatible<TNext>
    {
        public TNext SetTarget(Selector selector)
        {
            ArgumentNullException.ThrowIfNull(selector, nameof(selector));

            SetSyntax(selector.ToString());
            return TNext.Create(this);
        }

        public TNext SetTarget(Guid guid)
        {
            return SetTarget(guid.ToSelector());
        }

        public TNext SetTarget(Target target)
        {
            return SetTarget(target.ToCommandArgument());
        }

        public TNext SetTarget(string target)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));

            SetSyntax(target);
            return TNext.Create(this);
        }
    }
}
