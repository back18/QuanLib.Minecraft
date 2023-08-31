using QuanLib.Minecraft.Snbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Snbt.Data
{
    public class InteractionData
    {
        public InteractionData(Nbt nbt)
        {
            if (nbt is null)
                throw new ArgumentNullException(nameof(nbt));

            Player = SnbtSerializer.ToGuid(nbt.player);
            Timestamp = nbt.timestamp;
        }

        public InteractionData(Guid player, long timestamp)
        {
            Player = player;
            Timestamp = timestamp;
        }

        public Guid Player { get; }

        public long Timestamp { get; }

        public class Nbt
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public int[] player { get; set; }

            public long timestamp { get; set; }
        }
    }
}
