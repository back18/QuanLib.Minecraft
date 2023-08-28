using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Selectors
{
    public class TargetArgument<T> where T : notnull
    {
        public TargetArgument(T value, bool invert = false)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            MinValue = value;
            MaxValue = value;
            Invert = invert;
        }

        public TargetArgument(T minValue, T maxValue, bool invert = false)
        {
            MinValue = minValue ?? throw new ArgumentNullException(nameof(minValue));
            MaxValue = maxValue ?? throw new ArgumentNullException(nameof(maxValue));
            Invert = invert;
        }

        public T MinValue { get; }

        public T MaxValue { get; }

        public bool Invert { get; }

        public static implicit operator TargetArgument<T>(T value)
        {
            return new(value);
        }

        public string ToString(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"“{nameof(name)}”不能为 null 或空。", nameof(name));

            string? minValue, maxValue;
            if (MinValue is Gamemode minGamemode && MaxValue is Gamemode maxGamemode)
            {
                minValue = minGamemode.ToCommandArgument();
                maxValue = maxGamemode.ToCommandArgument();
            }
            else if (MinValue is Sort minSort && MaxValue is Sort maxSort)
            {
                minValue = minSort.ToCommandArgument();
                maxValue = maxSort.ToCommandArgument();
            }
            else
            {
                minValue = MinValue.ToString();
                maxValue = MaxValue.ToString();
            }

            string oper = Invert ? "=!" : "=";
            if (Equals(minValue, maxValue))
                return name + oper + minValue;
            return $"{name}{oper}{minValue}..{maxValue}";
        }
    }
}
