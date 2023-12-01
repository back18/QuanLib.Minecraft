using Newtonsoft.Json.Linq;
using QuanLib.Minecraft.Snbt.Parsers;
using QuanLib.Minecraft.Snbt.Tags;
using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Snbt
{
    public static class SnbtSerializer
    {
        private readonly static Type _boolType = typeof(bool);
        private readonly static Type _sbyteType = typeof(sbyte);
        private readonly static Type _dictionaryType = typeof(Dictionary<string, object>);

        public static T DeserializeObject<T>(string snbt)
        {
            return (T)DeserializeObject(snbt, typeof(T));
        }

        public static object DeserializeObject(string snbt, Type type)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(snbt, nameof(snbt));

            NbtCompoundParser parser = new();
            NbtCompound compound = parser.Parse(snbt);
            return DeserializeObject(compound, type);
        }

        private static object DeserializeObject(NbtCompound nbtCompound, Type type)
        {
            ArgumentNullException.ThrowIfNull(nbtCompound, nameof(nbtCompound));
            ArgumentNullException.ThrowIfNull(type, nameof(type));

            object obj = Activator.CreateInstance(type) ?? throw new NullReferenceException();

            Dictionary<string, PropertyInfo> propertys = new();
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string name;
                var att = property.GetCustomAttribute<SnbtPropertyAttribute>();
                if (att is null)
                    name = property.Name;
                else
                    name = att.PropertyName;
                propertys.Add(name, property);
            }

            Dictionary<string, FieldInfo> fields = new();
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                string name;
                var att = field.GetCustomAttribute<SnbtPropertyAttribute>();
                if (att is null)
                    name = field.Name;
                else
                    name = att.PropertyName;
                fields.Add(name, field);
            }

            foreach (var item in nbtCompound)
            {
                if (propertys.TryGetValue(item.Key, out var property))
                    property.SetValue(obj, GetValue(item.Value, property.PropertyType));
                else if (fields.TryGetValue(item.Key, out var field))
                    field.SetValue(obj, GetValue(item.Value, field.FieldType));
            }

            return obj;

            object GetValue(NbtTag tag, Type type)
            {
                object value;
                if (tag is NbtPrimitive primitive)
                {
                    if (type == _boolType && primitive.Value.GetType() == _sbyteType)
                        value = Convert.ToBoolean(primitive.Value);
                    else
                        value = primitive.Value;
                }
                else if (tag is NbtCompound compound)
                {
                    if (type == _dictionaryType)
                        value = DeserializeDictionary(compound);
                    else
                        value = DeserializeObject(compound, type);
                }
                else if (tag is NbtArray array)
                {
                    NbtTag[] tags = new NbtTag[array.Count];
                    array.CopyTo(tags, 0);
                    Type elementType = type.GetElementType() ?? throw new FormatException();
                    object[] objs = tags.Select(t => GetValue(t, elementType)).ToArray();
                    var values = Array.CreateInstance(elementType, objs.Length);
                    for (int i = 0; i < objs.Length; i++)
                        values.SetValue(objs[i], i);
                    value = values;
                }
                else
                    throw new InvalidOperationException();

                return value;
            }
        }

        public static Dictionary<string, object> DeserializeDictionary(NbtCompound nbtCompound)
        {
            ArgumentNullException.ThrowIfNull(nbtCompound, nameof(nbtCompound));

            Dictionary<string, object> result = new();
            foreach (var item in nbtCompound)
                result.Add(item.Key, GetValue(item.Value));

            return result;

            static object GetValue(NbtTag tag)
            {
                object value;
                if (tag is NbtPrimitive primitive)
                {
                    value = primitive.Value;
                }
                else if (tag is NbtCompound compound)
                {
                    value = DeserializeDictionary(compound);
                }
                else if (tag is NbtArray array)
                {
                    NbtTag[] tags = new NbtTag[array.Count];
                    array.CopyTo(tags, 0);
                    object[] objs = tags.Select(nbt => GetValue(nbt)).ToArray();
                    Array values;
                    if (objs.Length > 0)
                        values = Array.CreateInstance(objs[0].GetType(), objs.Length);
                    else
                        values = Array.Empty<object>();
                    for (int i = 0; i < objs.Length; i++)
                        values.SetValue(objs[i], i);
                    value = values;
                }
                else
                    throw new InvalidOperationException();

                return value;
            }
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
            => value != 0;

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
