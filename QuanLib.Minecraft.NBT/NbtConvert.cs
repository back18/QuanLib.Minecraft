using QuanLib.Core;
using QuanLib.Minecraft.NBT.SNBT;
using SharpNBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.NBT
{
    public static partial class NbtConvert
    {
        private static readonly Type _voidType = typeof(void);
        private static readonly Type _objectType = typeof(object);
        private static readonly Type _boolType = typeof(bool);
        private static readonly Type _sbyteType = typeof(sbyte);
        private static readonly Type _dictionaryType = typeof(Dictionary<string, object>);

        public static T DeserializeObject<T>(string snbt)
        {
            CompoundTag compoundTag = StringNbt.Parse(snbt);
            return DeserializeObject<T>(compoundTag);
        }

        public static object DeserializeObject(string snbt, Type type)
        {
            CompoundTag compoundTag = StringNbt.Parse(snbt);
            return DeserializeObject(compoundTag, type);
        }

        public static T DeserializeObject<T>(Tag tag)
        {
            return (T)DeserializeObject(tag, typeof(T));
        }

        public static object DeserializeObject(Tag tag, Type type)
        {
            ArgumentNullException.ThrowIfNull(tag, nameof(tag));
            ArgumentNullException.ThrowIfNull(type, nameof(type));

            if (tag is CompoundTag compoundTag)
            {
                if (type == _dictionaryType || type == _objectType)
                    return DeserializeDictionary(compoundTag);

                object value = Activator.CreateInstance(type) ?? throw new InvalidOperationException($"无法创建 {type} 类型的对象");
                Dictionary<string, SerializationMemberInfo> members = GetMembers(value);

                foreach (Tag childTag in compoundTag)
                {
                    if (childTag.Name is not null && members.TryGetValue(childTag.Name, out var member))
                        member.SetValue(value, DeserializeObject(childTag, member.MemberType));
                }

                return value;
            }
            else if (tag is ListTag listTag)
            {
                Type? elementType = type.GetElementType() ?? throw new ArgumentException($"无法获取 {type} 类型的元素类型");
                Array array = Array.CreateInstance(elementType, listTag.Count);

                for (int i = 0; i < listTag.Count; i++)
                {
                    object value = DeserializeObject(listTag[i], elementType);
                    array.SetValue(value, i);
                }

                Type arrayType = array.GetType();
                if (arrayType != type)
                    throw new InvalidOperationException($"无法将 {tag.Type} 类型的NBT标签转换为 {type} 类型");

                return array;
            }
            else
            {
                if (tag is ByteTag byteTag)
                {
                    if (type == _sbyteType)
                        return byteTag.SignedValue;
                    if (type == _boolType)
                        return byteTag.Bool;
                }

                object value = tag.AsValue() ?? throw new ArgumentException($"无法序列化 {tag.Type} 类型的NBT标签");
                Type valueType = value.GetType();

                if (valueType != type)
                    throw new InvalidOperationException($"无法将 {tag.Type} 类型的NBT标签转换为 {type} 类型");
                return value;
            }
        }

        public static Dictionary<string, object> DeserializeDictionary(CompoundTag compoundTag)
        {
            ArgumentNullException.ThrowIfNull(compoundTag, nameof(compoundTag));

            Dictionary<string, object> result = [];
            foreach (Tag childTag in compoundTag)
            {
                if (childTag.Name is null)
                    continue;

                if (childTag is CompoundTag compoundTag1)
                {
                    object value = DeserializeDictionary(compoundTag1);
                    result.Add(childTag.Name, value);
                }
                else if (childTag is ListTag listTag1)
                {
                    Type elementType = listTag1.ChildType.TypeOf();

                    Array array;
                    if (listTag1.Count == 0)
                    {
                        if (elementType == _voidType)
                            array = Array.Empty<object>();
                        else
                            array = Array.CreateInstance(elementType, 0);
                    }
                    else
                    {
                        array = Array.CreateInstance(elementType, listTag1.Count);
                        for (int i = 0; i < listTag1.Count; i++)
                        {
                            Tag tag = listTag1[i];

                            object value;
                            if (tag is CompoundTag compoundTag2)
                                value = DeserializeDictionary(compoundTag2);
                            else
                                value = DeserializeObject(tag, elementType);

                            array.SetValue(value, i);
                        }
                    }

                    result.Add(childTag.Name, array);
                }
                else
                {
                    object? value = childTag.AsValue();
                    if (value is null)
                        continue;

                    result.Add(childTag.Name, value);
                }
            }

            return result;
        }

        private static Dictionary<string, SerializationMemberInfo> GetMembers(object obj)
        {
            ArgumentNullException.ThrowIfNull(obj, nameof(obj));

            Dictionary<string, SerializationMemberInfo> result = [];
            List<SerializationMemberInfo> memberInfos = [];
            Type type = obj.GetType();
            memberInfos.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(s => new SerializationMemberInfo(s)));
            memberInfos.AddRange(type.GetFields(BindingFlags.Public | BindingFlags.Instance).Select(s => new SerializationMemberInfo(s)));

            foreach (var memberInfo in memberInfos)
            {
                string propertyName;
                NbtPropertyAttribute? attribute = memberInfo.MemberInfo.GetCustomAttribute<NbtPropertyAttribute>();
                if (attribute is null)
                    propertyName = memberInfo.MemberName;
                else
                    propertyName = FormatPropertyName(attribute.PropertyName, obj);

                result.Add(propertyName, memberInfo);
            }

            return result;
        }

        private static string FormatPropertyName(string propertyName, object obj)
        {
            ArgumentException.ThrowIfNullOrEmpty(propertyName, nameof(propertyName));
            ArgumentNullException.ThrowIfNull(obj, nameof(obj));

            Type type = obj.GetType();
            MatchCollection matches = MatcheBraces().Matches(propertyName);
            foreach (Match match in matches.Cast<Match>())
            {
                string memberName = match.Groups[1].Value;
                PropertyInfo? propertyInfo = type.GetProperty(memberName);
                FieldInfo? fieldInfo = type.GetField(memberName);
                if (propertyInfo is null && fieldInfo is null)
                    throw new InvalidOperationException($"无法从类型 {type} 获取名称为 {memberName} 的属性或字段");

                string value = propertyInfo?.GetValue(obj)?.ToString() ?? fieldInfo?.GetValue(obj)?.ToString() ?? string.Empty;
                propertyName = propertyName.Replace(match.Groups[0].Value, value);
            }

            return propertyName;
        }

        [GeneratedRegex(@"\{([^{}]+)\}")]
        private static partial Regex MatcheBraces();
    }
}
