using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.NBT
{
    public static class NbtUtil
    {
        public static bool TryParseEntityPosSbnt(string s, out EntityPos result)
        {
            if (string.IsNullOrEmpty(s))
                goto err;

            int index = s.IndexOf('[');
            if (index < 0)
                goto err;

            string[] items = s[index..].Trim('[', ']').Split(", "); ;
            if (items.Length != 3)
                goto err;

            if (!double.TryParse(items[0].TrimEnd('d'), out var x) ||
                !double.TryParse(items[1].TrimEnd('d'), out var y) ||
                !double.TryParse(items[2].TrimEnd('d'), out var z))
                goto err;

            result = new(x, y, z);
            return true;

            err:
            result = default;
            return false;
        }

        public static bool TryParseRotationSbnt(string s, out Rotation result)
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

        public static bool TryParseUuidSbnt(string s, out Guid result)
        {
            MatchCollection matches = Regex.Matches(s, @"-?\d+");

            List<int> items = new();
            foreach (Match match in matches.Cast<Match>())
            {
                if (!int.TryParse(match.Value, out var item))
                    goto err;

                items.Add(item);
            }

            if (items.Count != 4)
                goto err;

            result = ToGuid(items.ToArray());
            return true;

            err:
            result = default;
            return false;
        }

        public static Guid ToGuid(int[] value)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            if (value.Length != 4)
                throw new ArgumentException("UUID数组长度必须为4");

            byte[] i0 = BitConverter.GetBytes(value[0]);

            byte[] i1 = BitConverter.GetBytes(value[1]);
            byte b0 = i1[0];
            byte b1 = i1[1];
            i1[0] = i1[2];
            i1[1] = i1[3];
            i1[2] = b0;
            i1[3] = b1;

            byte[] i2 = BitConverter.GetBytes(value[2]);
            Array.Reverse(i2);

            byte[] i3 = BitConverter.GetBytes(value[3]);
            Array.Reverse(i3);

            return new Guid(i0.Concat(i1).Concat(i2).Concat(i3).ToArray());
        }

        public static bool ToBool(sbyte value)
        {
            return value != 0;
        }

        public static Vector3<T> ToVector3<T>(T[] value) where T : struct
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            if (value.Length != 3)
                throw new ArgumentException("三维向量数组长度必须为3");

            return new(value[0], value[1], value[2]);
        }

        public static Vector2<T> ToVector2<T>(T[] value) where T : struct
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            if (value.Length != 2)
                throw new ArgumentException("二维向量数组长度必须为2");

            return new(value[0], value[1]);
        }

        public static Rotation ToRotation(float[] value)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            if (value.Length != 2)
                throw new ArgumentException("Rotation数组长度必须为2");

            return new(value[0], value[1]);
        }
    }
}
