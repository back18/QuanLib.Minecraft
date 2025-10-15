using QuanLib.Minecraft.Command.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class IfScoreCompareSyntax(ICommandSyntax? previous) : CommandSyntax(previous)
    {
        private Selector? _targetA;
        private string? _objectiveA;
        private Selector? _targetB;
        private string? _objectiveB;
        private string? _operator;

        public IfScoreCompareSyntax SetTargetA(Selector selector)
        {
            ArgumentNullException.ThrowIfNull(selector, nameof(selector));

            _targetA = selector;
            return this;
        }

        public IfScoreCompareSyntax SetTargetA(Guid guid)
        {
            return SetTargetA(guid.ToSelector());
        }

        public IfScoreCompareSyntax SetTargetA(Target target)
        {
            return SetTargetA(target.ToCommandArgument());
        }

        public IfScoreCompareSyntax SetTargetA(string target)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));

            _targetA = new GenericSelector(target);
            return this;
        }

        public IfScoreCompareSyntax SetObjectiveA(string objective)
        {
            ArgumentException.ThrowIfNullOrEmpty(objective, nameof(objective));

            _objectiveA = objective;
            return this;
        }

        public IfScoreCompareSyntax SetTargetB(Selector selector)
        {
            ArgumentNullException.ThrowIfNull(selector, nameof(selector));

            _targetB = selector;
            return this;
        }

        public IfScoreCompareSyntax SetTargetB(Guid guid)
        {
            return SetTargetB(guid.ToSelector());
        }

        public IfScoreCompareSyntax SetTargetB(Target target)
        {
            return SetTargetB(target.ToCommandArgument());
        }

        public IfScoreCompareSyntax SetTargetB(string target)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));

            _targetB = new GenericSelector(target);
            return this;
        }

        public IfScoreCompareSyntax SetObjectiveB(string objective)
        {
            ArgumentException.ThrowIfNullOrEmpty(objective, nameof(objective));

            _objectiveB = objective;
            return this;
        }

        public IfScoreCompareSyntax SetOperator(string op)
        {
            ArgumentException.ThrowIfNullOrEmpty(op, nameof(op));
            if (op != "<" && op != ">" && op != "=" && op != "<=" && op != ">=")
                throw new ArgumentException("Operator must be one of: <, >, =, <=, >=", nameof(op));

            _operator = op;
            return this;
        }

        public ExecuteCommandSyntax EndIf()
        {
            if (_targetA is null)
                throw new InvalidOperationException("未设置目标A");
            if (string.IsNullOrEmpty(_objectiveA))
                throw new InvalidOperationException("未设置目标A的计分项");
            if (_targetB is null)
                throw new InvalidOperationException("未设置目标B");
            if (string.IsNullOrEmpty(_objectiveB))
                throw new InvalidOperationException("未设置目标B的计分项");
            if (string.IsNullOrEmpty(_operator))
                throw new InvalidOperationException("未设置操作符");

            SetSyntax($"{_targetA} {_objectiveA} {_operator} {_targetB} {_objectiveB}");
            return new ExecuteCommandSyntax(this);
        }
    }
}
