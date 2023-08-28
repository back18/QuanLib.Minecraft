using QuanLib.Minecraft.Snbt;
using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Snbt.Data
{
    public class Entity
    {
        public Entity(Nbt nbt)
        {
            if (nbt is null)
                throw new ArgumentNullException(nameof(nbt));

            Air = nbt.Air;
            CustomName = nbt.CustomName;
            CustomNameVisible = nbt.CustomNameVisible;
            FallDistance = nbt.FallDistance;
            Fire = nbt.Fire;
            Glowing = nbt.Glowing;
            HasVisualFire = nbt.HasVisualFire;
            Invulnerable = nbt.Invulnerable;
            Motion = SnbtSerializer.ToVector3(nbt.Motion);
            NoGravity = nbt.NoGravity is null ? null : nbt.NoGravity.Value;
            OnGround = nbt.OnGround;
            Passengers = nbt.Passengers;
            PortalCooldown = nbt.PortalCooldown;
            Pos = SnbtSerializer.ToVector3(nbt.Pos);
            Rotation = SnbtSerializer.ToRotation(nbt.Rotation);
            Silent = nbt.Silent;
            Tags = nbt.Tags;
            TicksFrozen = nbt.TicksFrozen;
            UUID = SnbtSerializer.ToGuid(nbt.UUID);
        }

        public short Air { get; }

        public string? CustomName { get; }

        public bool? CustomNameVisible { get; }

        public float FallDistance { get; }

        public short Fire { get; }

        public bool? Glowing { get; }

        public bool? HasVisualFire { get; }

        public bool Invulnerable { get; }

        public Vector3<double> Motion { get; }

        public bool? NoGravity { get; }

        public bool OnGround { get; }

        public IReadOnlyList<Entity>? Passengers { get; }

        public int PortalCooldown { get; }

        public Vector3<double> Pos { get; }

        public Rotation Rotation { get; }

        public bool? Silent { get; }

        public string[]? Tags { get; }

        public int? TicksFrozen { get; }

        public Guid UUID { get; }

        public class Nbt
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

            public short Air { get; set; }

            public string? CustomName { get; set; }

            public bool? CustomNameVisible { get; set; }

            public float FallDistance { get; set; }

            public short Fire { get; set; }

            public bool? Glowing { get; set; }

            public bool? HasVisualFire { get; set; }

            public bool Invulnerable { get; set; }

            public double[] Motion { get; set; }

            public bool? NoGravity { get; set; }

            public bool OnGround { get; set; }

            public Entity[]? Passengers { get; set; }

            public int PortalCooldown { get; set; }

            public double[] Pos { get; set; }

            public float[] Rotation { get; set; }

            public bool? Silent { get; set; }

            public string[]? Tags { get; set; }

            public int? TicksFrozen { get; set; }

            public int[] UUID { get; set; }
        }
    }
}
