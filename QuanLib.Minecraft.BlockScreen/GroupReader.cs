using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public abstract class GroupReader : IMCOSComponent
    {
        public MCOS MCOS
        {
            get
            {
                if (_MCOS is null)
                    throw new InvalidOperationException();
                return _MCOS;
            }
            internal set => _MCOS = value;
        }
        private MCOS? _MCOS;

        public abstract void Read();
    }
}
