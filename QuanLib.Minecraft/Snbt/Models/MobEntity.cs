using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Snbt.Models
{
    public class MobEntity : Entity
    {
        public MobEntity(Model model) : base(model)
        {
            AbsorptionAmount = model.AbsorptionAmount;
            ActiveEffects = model.ActiveEffects is null ? null : new(model.ActiveEffects);
            Attributes = new(model.Attributes);
            Brain = model.Brain;
            DeathTime = model.DeathTime;
            FallFlying = model.FallFlying;
            Health = model.Health;
            HurtByTimestamp = model.HurtByTimestamp;
            HurtTime = model.HurtTime;
            Sleeping = new(model.SleepingX, model.SleepingY, model.SleepingZ);
        }

        public float AbsorptionAmount { get; }

        public ReadOnlyCollection<object>? ActiveEffects { get; }

        public ReadOnlyCollection<object> Attributes { get; }

        public object Brain { get; }

        public short DeathTime { get; }

        public bool FallFlying { get; }

        public float Health { get; }

        public int HurtByTimestamp { get; }

        public short HurtTime { get; }

        public BlockPos Sleeping { get; }

        public new class Model : Entity.Model
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
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }
    }
}
