using QuanLib.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class IfBlocksCommandSyntax(ICommandSyntax? previous) : CommandSyntax(previous)
    {
        private IVector3<int>? _start;
        private IVector3<int>? _end;
        private IVector3<int>? _destination;
        private string? _mode;

        public IfBlocksCommandSyntax SetStart<T>(T start) where T : IVector3<int>
        {
            ArgumentNullException.ThrowIfNull(start, nameof(start));

            _start = start;
            return this;
        }

        public IfBlocksCommandSyntax SetEnd<T>(T end) where T : IVector3<int>
        {
            ArgumentNullException.ThrowIfNull(end, nameof(end));

            _end = end;
            return this;
        }

        public IfBlocksCommandSyntax SetDestination<T>(T destination) where T : IVector3<int>
        {
            ArgumentNullException.ThrowIfNull(destination, nameof(destination));

            _destination = destination;
            return this;
        }

        public IfBlocksCommandSyntax SetStart(int x, int y, int z)
        {
            _start = new Vector3<int>(x, y, z);
            return this;
        }

        public IfBlocksCommandSyntax SetEnd(int x, int y, int z)
        {
            _end = new Vector3<int>(x, y, z);
            return this;
        }

        public IfBlocksCommandSyntax SetDestination(int x, int y, int z)
        {
            _destination = new Vector3<int>(x, y, z);
            return this;
        }

        public IfBlocksCommandSyntax SetAllMode()
        {
            _mode = "all";
            return this;
        }

        public IfBlocksCommandSyntax SetMaskedMode()
        {
            _mode = "masked";
            return this;
        }

        public ExecuteCommandSyntax EndIf()
        {
            if (_start is null)
                throw new InvalidOperationException("起始位置未设置");
            if (_end is null)
                throw new InvalidOperationException("结束位置未设置");
            if (_destination is null)
                throw new InvalidOperationException("目标位置未设置");
            if (string.IsNullOrEmpty(_mode))
                throw new InvalidOperationException("模式未设置");

            StringBuilder syntaxBuilder = new();
            syntaxBuilder.AppendFormat("{0} {1} {2} ", _start.X, _start.Y, _start.Z);
            syntaxBuilder.AppendFormat("{0} {1} {2} ", _end.X, _end.Y, _end.Z);
            syntaxBuilder.AppendFormat("{0} {1} {2} ", _destination.X, _destination.Y, _destination.Z);
            syntaxBuilder.Append(_mode);
            SetSyntax(syntaxBuilder.ToString());
            return new ExecuteCommandSyntax(this);
        }
    }
}
