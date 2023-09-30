using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Snbt.Models
{
    public class Entity
    {
        public Entity(Model model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            Air = model.Air;
            CustomName = model.CustomName;
            CustomNameVisible = model.CustomNameVisible;
            FallDistance = model.FallDistance;
            Fire = model.Fire;
            Glowing = model.Glowing;
            HasVisualFire = model.HasVisualFire;
            Invulnerable = model.Invulnerable;
            Motion = SnbtSerializer.ToVector3(model.Motion);
            NoGravity = model.NoGravity is null ? null : model.NoGravity.Value;
            OnGround = model.OnGround;
            Passengers = model.Passengers;
            PortalCooldown = model.PortalCooldown;
            Pos = SnbtSerializer.ToVector3(model.Pos);
            Rotation = SnbtSerializer.ToRotation(model.Rotation);
            Silent = model.Silent;
            Tags = model.Tags;
            TicksFrozen = model.TicksFrozen;
            UUID = SnbtSerializer.ToGuid(model.UUID);
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

        public class Model
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
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }
    }
}
