using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Vectors
{
    public struct Vector2<T> : IVector2<T>
    {
        public Vector2(T x, T y)
        {
            X = x;
            Y = y;
        }

        public T X { get; set; }

        public T Y { get; set; }
    }
}
