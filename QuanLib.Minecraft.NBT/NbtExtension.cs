using SharpNBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.NBT
{
    public static class NbtExtension
    {
        static NbtExtension()
        {
            _typeMap = new()
            {
                { TagType.String, typeof(string) },
                { TagType.Byte, typeof(byte) },
                { TagType.Short, typeof(short) },
                { TagType.Int, typeof(int) },
                { TagType.Long, typeof(long) },
                { TagType.Float, typeof(float) },
                { TagType.Double, typeof(double) },
                { TagType.ByteArray, typeof(byte[]) },
                { TagType.IntArray, typeof(int[]) },
                { TagType.LongArray, typeof(long[]) },
                { TagType.List, typeof(object[]) },
                { TagType.Compound, typeof(object) },
                { TagType.End, typeof(void) }
            };
        }

        private static readonly Dictionary<TagType, Type> _typeMap;

        public static Type TypeOf(this TagType source)
        {
            return _typeMap[source];
        }

        public static string? AsStringValue(this Tag source)
        {
            if (source is StringTag stringTag)
                return stringTag.Value;
            else
                return null;
        }

        public static object? AsNumberValue(this Tag source)
        {
            if (source is ByteTag byteTag)
                return byteTag.Value;
            else if (source is ShortTag shortTag)
                return shortTag.Value;
            else if (source is IntTag intTag)
                return intTag.Value;
            else if (source is LongTag longTag)
                return longTag.Value;
            else if (source is FloatTag floatTag)
                return floatTag.Value;
            else if (source is DoubleTag doubleTag)
                return doubleTag.Value;
            else
                return null;
        }

        public static Array? AsArrayValue(this Tag source)
        {
            if (source is ByteArrayTag byteArrayTag)
            {
                byte[] result = byteArrayTag;
                return result;
            }
            else if (source is IntArrayTag intArrayTag)
            {
                int[] result = intArrayTag;
                return result;
            }
            else if (source is LongArrayTag longArrayTag)
            {
                long[] result = longArrayTag;
                return result;
            }
            else
            {
                return null;
            }
        }

        public static object? AsValue(this Tag source)
        {
            object? value;

            value = source.AsStringValue();
            if (value is not null)
                return value;

            value = source.AsNumberValue();
            if (value is not null)
                return value;

            value = source.AsArrayValue();
            if (value is not null)
                return value;

            return null;
        }
    }
}
