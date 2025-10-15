using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class AlignSyntax(ICommandSyntax? previous) : CommandSyntax(previous)
    {
        private string _syntax = string.Empty;

        public AlignSyntax SetX()
        {
            if (!_syntax.Contains('x'))
            {
                _syntax += 'x';
                SetSyntax(_syntax);
            }
            return this;
        }

        public AlignSyntax SetY()
        {
            if (!_syntax.Contains('y'))
            {
                _syntax += 'y';
                SetSyntax(_syntax);
            }
            return this;
        }

        public AlignSyntax SetZ()
        {
            if (!_syntax.Contains('Z'))
            {
                _syntax += 'z';
                SetSyntax(_syntax);
            }
            return this;
        }

        public ExecuteCommandSyntax EndAlign()
        {
            if (string.IsNullOrEmpty(_syntax))
                throw new InvalidOperationException("未设置对齐轴");
            return new ExecuteCommandSyntax(this);
        }
    }
}
