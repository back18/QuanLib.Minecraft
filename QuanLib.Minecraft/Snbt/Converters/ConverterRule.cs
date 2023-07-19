using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft.Snbt.Converters
{
    public abstract class ConverterRule
    {
        public abstract Type InputType { get; }
        public abstract Type ResultType { get; }

        public abstract bool TryConvertGeneric(object value, out object result);
    }
    public class ConverterRule<T, R> : ConverterRule
    {
        public delegate bool ConvertFunc(T input, out R result);

        private ConvertFunc ConvertFuncInstance { get; }

        public override Type InputType { get; }
        public override Type ResultType { get; }

        public ConverterRule(ConvertFunc convert)
        {
            ConvertFuncInstance = convert;
            InputType = typeof(T);
            ResultType = typeof(R);
        }

        public ConverterRule(Func<T, R> convert) : this((T v, out R r) =>
        {
            r = convert(v);
            return true;
        })
        { }

        public override bool TryConvertGeneric(object value, out object result)
        {
            var boolResult = ConvertFuncInstance((T)value, out var typedResult);
            result = typedResult;
            return boolResult;
        }

        public bool TryConvert(T value, out R result)
        {
            return ConvertFuncInstance(value, out result);
        }
    }
}
