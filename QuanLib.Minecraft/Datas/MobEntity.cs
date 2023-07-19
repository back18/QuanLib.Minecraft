using QuanLib.Minecraft.Snbt;
using QuanLib.Minecraft.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Datas
{
    public class MobEntity : Entity
    {
        public MobEntity(Nbt nbt) : base(nbt)
        {
            AbsorptionAmount = nbt.AbsorptionAmount;
            ActiveEffects = nbt.ActiveEffects;
            Attributes = nbt.Attributes;
            Brain = nbt.Brain;
            DeathTime = nbt.DeathTime;
            FallFlying = nbt.FallFlying;
            Health = nbt.Health;
            HurtByTimestamp = nbt.HurtByTimestamp;
            HurtTime = nbt.HurtTime;
            Sleeping = new(nbt.SleepingX, nbt.SleepingY, nbt.SleepingZ);
        }

        public float AbsorptionAmount { get; }

        public IReadOnlyList<object>? ActiveEffects { get; }

        public IReadOnlyList<object> Attributes { get; }

        public object Brain { get; }

        public short DeathTime { get; }

        public bool FallFlying { get; }

        public float Health { get; }

        public int HurtByTimestamp { get; }

        public short HurtTime { get; }

        public Vector3<int> Sleeping { get; }

        public new class Nbt : Entity.Nbt
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

            public float AbsorptionAmount { get; set; }

            public object[]? ActiveEffects { get; set; }

            public object[] Attributes { get; set; }

            public object Brain { get; set; }

            public short DeathTime { get; set; }

            public bool FallFlying { get; set; }

            public float Health { get; set; }

            public int HurtByTimestamp { get; set; }

            public short HurtTime { get; set; }

            public int SleepingX { get; set; }

            public int SleepingY { get; set; }

            public int SleepingZ { get; set; }
        }
    }
}
