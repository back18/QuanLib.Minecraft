using Newtonsoft.Json.Linq;
using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
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
            readonly get { return _Yaw; }
            set { _Yaw = Math.Clamp(value, -180f, 180f); }
        }
        private float _Yaw;

        /// <summary>
        /// 俯仰角
        /// </summary>
        public float Pitch
        {
            readonly get { return _Pitch; }
            set { _Pitch = Math.Clamp(value, -90f, 90f); }
        }
        private float _Pitch;

        public readonly Facing YawFacing
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

        public readonly Facing PitchFacing => _Pitch < 0 ? Facing.Yp : Facing.Ym;

        public readonly bool Contains(Facing facing)
        {
            return facing switch
            {
                Facing.Zp => _Yaw > -90 && _Yaw <= 90,
                Facing.Zm => _Yaw <= -90 || _Yaw > 90,
                Facing.Xp => _Yaw < 0,
                Facing.Xm => _Yaw >= 0,
                Facing.Yp => _Pitch < 0,
                Facing.Ym => _Pitch >= 0,
                _ => throw new InvalidOperationException(),
            };
        }

        public readonly Vector3<double> ToDirection()
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
    }
}
