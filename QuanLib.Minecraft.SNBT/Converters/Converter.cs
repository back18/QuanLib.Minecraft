using System;
using System.Collections.Generic;
using System.Globalization;
using C = System.Convert;

namespace QuanLib.Minecraft.SNBT.Converters
{
    /// <summary>
    /// This class only exists because <see cref="System.Convert"/> is terrible.
    /// </summary>
    public class Converter
    {
        private static Converter defaultSingleton;
        public static Converter Default
        {
            get
            {
                if (defaultSingleton == null)
                {
                    defaultSingleton = new Converter(
                        new ConverterRule<sbyte, bool>(v => C.ToBoolean(v)),
                        new ConverterRule<short, bool>(v => C.ToBoolean(v)),
                        new ConverterRule<int, bool>(v => C.ToBoolean(v)),
                        new ConverterRule<long, bool>(v => C.ToBoolean(v)),

                        new ConverterRule<bool, sbyte>((bool v, out sbyte r) => TrySystemConvert(() => C.ToSByte(v), out r)),
                        new ConverterRule<short, sbyte>((short v, out sbyte r) => TrySystemConvert(() => C.ToSByte(v), out r)),
                        new ConverterRule<int, sbyte>((int v, out sbyte r) => TrySystemConvert(() => C.ToSByte(v), out r)),
                        new ConverterRule<long, sbyte>((long v, out sbyte r) => TrySystemConvert(() => C.ToSByte(v), out r)),

                        new ConverterRule<bool, short>((bool v, out short r) => TrySystemConvert(() => C.ToInt16(v), out r)),
                        new ConverterRule<sbyte, short>((sbyte v, out short r) => TrySystemConvert(() => C.ToInt16(v), out r)),
                        new ConverterRule<int, short>((int v, out short r) => TrySystemConvert(() => C.ToInt16(v), out r)),
                        new ConverterRule<long, short>((long v, out short r) => TrySystemConvert(() => C.ToInt16(v), out r)),

                        new ConverterRule<bool, int>((bool v, out int r) => TrySystemConvert(() => C.ToInt32(v), out r)),
                        new ConverterRule<sbyte, int>((sbyte v, out int r) => TrySystemConvert(() => C.ToInt32(v), out r)),
                        new ConverterRule<short, int>((short v, out int r) => TrySystemConvert(() => C.ToInt32(v), out r)),
                        new ConverterRule<long, int>((long v, out int r) => TrySystemConvert(() => C.ToInt32(v), out r)),

                        new ConverterRule<bool, long>((bool v, out long r) => TrySystemConvert(() => C.ToInt64(v), out r)),
                        new ConverterRule<sbyte, long>((sbyte v, out long r) => TrySystemConvert(() => C.ToInt64(v), out r)),
                        new ConverterRule<short, long>((short v, out long r) => TrySystemConvert(() => C.ToInt64(v), out r)),
                        new ConverterRule<int, long>((int v, out long r) => TrySystemConvert(() => C.ToInt64(v), out r)),

                        new ConverterRule<bool, float>((bool v, out float r) => TrySystemConvert(() => C.ToSingle(v), out r)),
                        new ConverterRule<sbyte, float>((sbyte v, out float r) => TrySystemConvert(() => C.ToSingle(v), out r)),
                        new ConverterRule<short, float>((short v, out float r) => TrySystemConvert(() => C.ToSingle(v), out r)),
                        new ConverterRule<int, float>((int v, out float r) => TrySystemConvert(() => C.ToSingle(v), out r)),
                        new ConverterRule<long, float>((long v, out float r) => TrySystemConvert(() => C.ToSingle(v), out r)),
                        new ConverterRule<double, float>((double v, out float r) => TrySystemConvert(() => C.ToSingle(v), out r)),

                        new ConverterRule<bool, double>((bool v, out double r) => TrySystemConvert(() => C.ToDouble(v), out r)),
                        new ConverterRule<sbyte, double>((sbyte v, out double r) => TrySystemConvert(() => C.ToDouble(v), out r)),
                        new ConverterRule<short, double>((short v, out double r) => TrySystemConvert(() => C.ToDouble(v), out r)),
                        new ConverterRule<int, double>((int v, out double r) => TrySystemConvert(() => C.ToDouble(v), out r)),
                        new ConverterRule<long, double>((long v, out double r) => TrySystemConvert(() => C.ToDouble(v), out r)),
                        new ConverterRule<float, double>((float v, out double r) => TrySystemConvert(() => C.ToDouble(v), out r))
                    );
                }
                return defaultSingleton;
            }
        }

        private Dictionary<Type, Dictionary<Type, ConverterRule>> Registry { get; } = new Dictionary<Type, Dictionary<Type, ConverterRule>>();

        public Converter(params ConverterRule[] rules)
        {
            foreach (var rule in rules)
            {
                if (!Registry.TryGetValue(rule.InputType, out var resultMap))
                {
                    resultMap = new Dictionary<Type, ConverterRule>();
                    Registry.Add(rule.InputType, resultMap);
                }
                resultMap.Add(rule.ResultType, rule);
            }
        }

        private static bool TrySystemConvert<R>(Func<R> action, out R result)
        {
            try
            {
                result = action();
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        public R Convert<R>(object value)
        {
            if (TryConvert(value, out R result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException($"Could not convert {value} from {value.GetType().FullName} to {typeof(R).FullName}");
            }
        }

        public bool TryConvert<R>(object value, out R result)
        {
            object genericResult = null;
            var boolReturn = Registry.TryGetValue(value.GetType(), out var resultMap) && resultMap.TryGetValue(typeof(R), out var rule) && rule.TryConvertGeneric(value, out genericResult);
            result = boolReturn ? (R)genericResult : default;
            return boolReturn;
        }
    }
}
