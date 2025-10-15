using QuanLib.Minecraft.Command.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class IfEntityDataSyntax(ICommandSyntax? previous) : CommandSyntax(previous)
    {
        private Selector? _target;
        private string? _nbtPath;

        public IfEntityDataSyntax SetTarget(Selector selector)
        {
            ArgumentNullException.ThrowIfNull(selector, nameof(selector));

            _target = selector;
            return this;
        }

        public IfEntityDataSyntax SetTarget(Guid guid)
        {
            return SetTarget(guid.ToSelector());
        }

        public IfEntityDataSyntax SetTarget(Target target)
        {
            return SetTarget(target.ToCommandArgument());
        }

        public IfEntityDataSyntax SetTarget(string target)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));

            _target = new GenericSelector(target);
            return this;
        }

        public IfEntityDataSyntax SetNbtPath(string nbtPath)
        {
            ArgumentNullException.ThrowIfNull(nbtPath, nameof(nbtPath));

            _nbtPath = nbtPath;
            return this;
        }

        public ExecuteCommandSyntax EndIf()
        {
            if (_target == null)
                throw new InvalidOperationException("目标未设置");
            if (string.IsNullOrEmpty(_nbtPath))
                throw new InvalidOperationException("NBT路径未设置");

            SetSyntax($"{_target} {_nbtPath}");
            return new ExecuteCommandSyntax(this);
        }
    }
}
