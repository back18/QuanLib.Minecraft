using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Snbt.Data
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

        public Item(sbyte count, string iD, sbyte slot, Dictionary<string, object>? tag = null)
        {
            Count = count;
            ID = iD;
            Slot = slot;
            Tag = tag;
        }

        public sbyte Count { get; }

        public string ID { get; }

        public sbyte Slot { get; }

        public Dictionary<string, object>? Tag { get; }

        public override string ToString()
        {
            return ID;
        }

        public static bool EqualsID(Item? item1, Item? item2)
        {
            return string.Equals(item1?.ID, item2?.ID);
        }

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
