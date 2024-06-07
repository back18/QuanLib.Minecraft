using QuanLib.Core;
using QuanLib.Game;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.NBT.Models
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

        public Vector3<int> Sleeping { get; }

        public new class Model : Entity.Model
        {
            public required float AbsorptionAmount { get; set; }

            [Nullable]
            public object[]? ActiveEffects { get; set; }

            public required object[] Attributes { get; set; }

            public required object Brain { get; set; }

            public required short DeathTime { get; set; }

            public required bool FallFlying { get; set; }

            public required float Health { get; set; }

            public required int HurtByTimestamp { get; set; }

            public required short HurtTime { get; set; }

            public required int SleepingX { get; set; }

            public required int SleepingY { get; set; }

            public required int SleepingZ { get; set; }
        }
    }
}
