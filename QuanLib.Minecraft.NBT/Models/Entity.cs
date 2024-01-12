using QuanLib.Core;
using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.NBT.Models
{
    public class Entity
    {
        public Entity(Model model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            Air = model.Air;
            CustomName = model.CustomName;
            CustomNameVisible = model.CustomNameVisible;
            FallDistance = model.FallDistance;
            Fire = model.Fire;
            Glowing = model.Glowing;
            HasVisualFire = model.HasVisualFire;
            Invulnerable = model.Invulnerable;
            Motion = NbtUtil.ToVector3(model.Motion);
            NoGravity = model.NoGravity is null ? null : model.NoGravity.Value;
            OnGround = model.OnGround;
            Passengers = model.Passengers is null ? null : new(model.Passengers);
            PortalCooldown = model.PortalCooldown;
            Pos = NbtUtil.ToVector3(model.Pos);
            Rotation = NbtUtil.ToRotation(model.Rotation);
            Silent = model.Silent;
            Tags = model.Tags is null ? null : new(model.Tags);
            TicksFrozen = model.TicksFrozen;
            UUID = NbtUtil.ToGuid(model.UUID);
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

        public ReadOnlyCollection<Entity>? Passengers { get; }

        public int PortalCooldown { get; }

        public Vector3<double> Pos { get; }

        public Rotation Rotation { get; }

        public bool? Silent { get; }

        public ReadOnlyCollection<string>? Tags { get; }

        public int? TicksFrozen { get; }

        public Guid UUID { get; }

        public class Model
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

            public short Air { get; set; }

            [Nullable]
            public string? CustomName { get; set; }

            [Nullable]
            public bool? CustomNameVisible { get; set; }

            public float FallDistance { get; set; }

            public short Fire { get; set; }

            [Nullable]
            public bool? Glowing { get; set; }

            [Nullable]
            public bool? HasVisualFire { get; set; }

            public bool Invulnerable { get; set; }

            public double[] Motion { get; set; }

            [Nullable]
            public bool? NoGravity { get; set; }

            public bool OnGround { get; set; }

            [Nullable]
            public Entity[]? Passengers { get; set; }

            public int PortalCooldown { get; set; }

            public double[] Pos { get; set; }

            public float[] Rotation { get; set; }

            [Nullable]
            public bool? Silent { get; set; }

            [Nullable]
            public string[]? Tags { get; set; }

            [Nullable]
            public int? TicksFrozen { get; set; }

            public int[] UUID { get; set; }

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }
    }
}
