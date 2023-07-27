using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Datas
{
    public class Item
    {
        public Item(Nbt nbt)
        {
            if (nbt is null)
                throw new ArgumentNullException(nameof(nbt));

            Count = nbt.Count;
            ID = nbt.id;
            Slot = nbt.Slot;
            Tag = nbt.tag;
        }

        public sbyte Count { get; }

        public string ID { get; }

        public sbyte Slot { get; }

        public Dictionary<string, object>? Tag { get; }

        public class Nbt
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public sbyte Count { get; set; }

            public string id { get; set; }

            public sbyte Slot { get; set; }

            public Dictionary<string, object>? tag { get; set; }
        }
    }
}
