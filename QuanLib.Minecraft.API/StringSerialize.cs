using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public abstract class StringSerialize : ISerialize
    {
        protected StringSerialize()
        {
            Encoding = Encoding.UTF8;
        }

        protected StringSerialize(Encoding encoding)
        {
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        public virtual Encoding Encoding { get; }

        public abstract byte[] Serialize();
    }
}
