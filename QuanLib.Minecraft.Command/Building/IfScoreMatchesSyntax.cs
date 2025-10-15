using QuanLib.Minecraft.Command.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class IfScoreMatchesSyntax(ICommandSyntax? previous) : CommandSyntax(previous)
    {
        private Selector? _target;
        private string? _objective;
        private string? _range;

        public IfScoreMatchesSyntax SetTarget(Selector selector)
        {
            ArgumentNullException.ThrowIfNull(selector, nameof(selector));

            _target = selector;
            return this;
        }

        public IfScoreMatchesSyntax SetTarget(Guid guid)
        {
            return SetTarget(guid.ToSelector());
        }

        public IfScoreMatchesSyntax SetTarget(Target target)
        {
            return SetTarget(target.ToCommandArgument());
        }

        public IfScoreMatchesSyntax SetTarget(string target)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));

            _target = new GenericSelector(target);
            return this;
        }

        public IfScoreMatchesSyntax SetObjective(string objective)
        {
            ArgumentException.ThrowIfNullOrEmpty(objective, nameof(objective));

            _objective = objective;
            return this;
        }

        public IfScoreMatchesSyntax SetRange(int start, int end)
        {
            if (end < start)
                throw new ArgumentException("End must be greater than or equal to start.", nameof(end));

            _range = start == end ? start.ToString() : $"{start}..{end}";
            return this;
        }

        public ExecuteCommandSyntax EndIf()
        {
            if (_target == null)
                throw new InvalidOperationException("未设置目标");
            if (string.IsNullOrEmpty(_objective))
                throw new InvalidOperationException("未设置目标分数项");
            if (string.IsNullOrEmpty(_range))
                throw new InvalidOperationException("未设置分数范围");

            SetSyntax($"{_target} {_objective} matches {_range}");
            return new ExecuteCommandSyntax(this);
        }
    }
}
