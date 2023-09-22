using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Snbt.Model
{
    public class InteractionData
    {
        public InteractionData(Model model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            Player = SnbtSerializer.ToGuid(model.player);
            Timestamp = model.timestamp;
        }

        public InteractionData(Guid player, long timestamp)
        {
            Player = player;
            Timestamp = timestamp;
        }

        public static readonly InteractionData Empty = new(Guid.Empty, 0);

        public Guid Player { get; }

        public long Timestamp { get; }

        public class Model
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public int[] player { get; set; }

            public long timestamp { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }
    }
}
