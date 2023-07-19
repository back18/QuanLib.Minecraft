using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Vectors
{
    public interface IVector3<T>
    {
        public T X { get; }

        public T Y { get; }

        public T Z { get; }
    }
}
