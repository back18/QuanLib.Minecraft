using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.Command;

namespace QuanLib.Minecraft.Selectors
{
    public class TargetArgument<T> where T : notnull
    {
        public TargetArgument(T value, bool invert = false)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));

            MinValue = value;
            MaxValue = value;
            Invert = invert;
        }

        public TargetArgument(T minValue, T maxValue, bool invert = false)
        {
            ArgumentNullException.ThrowIfNull(maxValue, nameof(maxValue));
            ArgumentNullException.ThrowIfNull(minValue, nameof(minValue));

            MinValue = minValue;
            MaxValue = maxValue;
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
            ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));

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
