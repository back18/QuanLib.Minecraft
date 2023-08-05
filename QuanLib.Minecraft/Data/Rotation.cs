﻿using Newtonsoft.Json.Linq;
using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Data
{
    public struct Rotation
    {
        public Rotation(float yaw, float pitch)
        {
            _Yaw = Math.Clamp(yaw, -180f, 180f);
            _Pitch = Math.Clamp(pitch, -90f, 90f);
        }

        /// <summary>
        /// 偏转角
        /// </summary>
        public float Yaw
        {
            get { return _Yaw; }
            set { _Yaw = Math.Clamp(value, -180f, 180f); }
        }
        private float _Yaw;

        /// <summary>
        /// 俯仰角
        /// </summary>
        public float Pitch
        {
            get { return _Pitch; }
            set { _Pitch = Math.Clamp(value, -90f, 90f); }
        }
        private float _Pitch;

        public Facing YawFacing
        {
            get
            {
                if (_Yaw < -135)
                    return Facing.Zm;
                else if (_Yaw < -45)
                    return Facing.Xp;
                else if (_Yaw < 45)
                    return Facing.Zp;
                else if (_Yaw < 135)
                    return Facing.Xm;
                else
                    return Facing.Zm;
            }
        }

        public Facing PitchFacing => _Pitch < 0 ? Facing.Yp : Facing.Ym;

        public bool Contain(Facing facing)
        {
            switch (facing)
            {
                case Facing.Zp:
                    return _Yaw > -90 && _Yaw <= 90;
                case Facing.Zm:
                    return _Yaw <= -90 || _Yaw > 90;
                case Facing.Xp:
                    return _Yaw < 0;
                case Facing.Xm:
                    return _Yaw >= 0;
                case Facing.Yp:
                    return _Pitch < 0;
                case Facing.Ym:
                    return _Pitch >= 0;
                default:
                    throw new InvalidOperationException();
            }
        }

        public Vector3Double ToDirection()
        {
            float yaw = _Yaw;
            float pitch = _Pitch;
            if (yaw < 0)
                yaw += 360;
            yaw = -yaw;
            pitch = -pitch;

            double radiansYaw = yaw * (MathF.PI / 180);
            double radiansPitch = pitch * (MathF.PI / 180);

            double dirX = Math.Cos(radiansPitch) * Math.Sin(radiansYaw);
            double dirY = Math.Sin(radiansPitch);
            double dirZ = Math.Cos(radiansPitch) * Math.Cos(radiansYaw);

            return new(dirX, dirY, dirZ);
        }

        public static bool TryParse(string s, out Rotation result)
        {
            if (string.IsNullOrEmpty(s))
                goto err;

            int index = s.IndexOf('[');
            if (index < 0)
                goto err;

            string[] items = s[index..].Trim('[', ']').Split(", "); ;
            if (items.Length != 2)
                goto err;

            if (!float.TryParse(items[0].TrimEnd('f'), out var yaw) ||
                !float.TryParse(items[1].TrimEnd('f'), out var pitch))
                goto err;

            result = new(yaw, pitch);
            return true;

            err:
            result = default;
            return false;
        }
    }
}
