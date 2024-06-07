using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.NBT.Models
{
    public class InteractionData
    {
        public InteractionData(Model model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            Player = NbtUtil.ToGuid(model.player);
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
            public required int[] player { get; set; }

            public required long timestamp { get; set; }
        }
    }
}
