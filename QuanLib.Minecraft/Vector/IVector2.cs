using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Vector
{
    public interface IVector2<T>
    {
        public T X { get; }

        public T Y { get; }
    }
}
