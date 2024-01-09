using QuanLib.Minecraft.SNBT.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft.SNBT.Tags
{
    /// <summary>
    /// Base class for primitives:
    /// <list type="bullet">
    /// <item><see cref="bool"/></item>
    /// <item><see cref="sbyte"/></item>
    /// <item><see cref="short"/></item>
    /// <item><see cref="int"/></item>
    /// <item><see cref="long"/></item>
    /// <item><see cref="float"/></item>
    /// <item><see cref="double"/></item>
    /// <item><see cref="string"/></item>
    /// </list>
    /// Type can be used to downcast from <see cref="NbtTag"/> without knowing the exact value type,
    /// which would be required if you used <see cref="NbtPrimitive{T}"/>
    /// </summary>
    public abstract class NbtPrimitive : NbtTag
    {
        public abstract object Value { get; }

        /// <summary>
        /// Checks if the value type is exactly <typeparamref name="R"/>.
        /// </summary>
        /// <typeparam name="R">The type to compare the internal value with.</typeparam>
        /// <returns>Returns true if the internal value is of type <typeparamref name="R"/></returns>
        public abstract bool ValueIsExact<R>();

        /// <summary>
        /// Returns whether or not the value can be retrieved as <typeparamref name="R"/> and if so, the converted value.
        /// </summary>
        /// <typeparam name="R">The type to check for and return.</typeparam>
        /// <param name="result">If the value can be returned as <typeparamref name="R"/>, this variable will contain the result.</param>
        /// <returns>Returns true if the value can be returned as <typeparamref name="R"/></returns>
        public abstract bool TryValueAs<R>(out R result);

        /// <summary>
        /// Returns the value as <typeparamref name="R"/> or throws <see cref="InvalidCastException"/>.
        /// </summary>
        /// <typeparam name="R">The type to return the value as.</typeparam>
        /// <exception cref="InvalidCastException">Thrown if the value cannot be converted.</exception>
        /// <returns>Returns <typeparamref name="R"/> if possible.</returns>
        public abstract R ValueAs<R>();

        /// <summary>
        /// Checks if the internal value equals another value, converting the internal value to the specified type first.
        /// </summary>
        /// <typeparam name="R">The type to use when making the comparison.</typeparam>
        /// <param name="value">The other value to compare with.</param>
        /// <returns>Returns true if the internal value can be converted to <typeparamref name="R"/> and equals the given value.</returns>
        public abstract bool ValueEquals<R>(R value);
    }

    /// <summary>
    /// Containing class for primitives.
    /// See also: <see cref="NbtPrimitive"/>
    /// </summary>
    /// <typeparam name="T">The raw value type.</typeparam>
    public class NbtPrimitive<T> : NbtPrimitive where T : notnull
    {
        private T RawValue { get; }

        public NbtPrimitive(T rawValue)
        {
            RawValue = rawValue;
        }

        public override object Value => RawValue;

        public override bool ValueIsExact<R>()
        {
            return RawValue is R;
        }

        public override R ValueAs<R>()
        {
            if (typeof(T) == typeof(R))
            {
                return (R)(object)RawValue;
            }
            else
            {
                return Converter.Default.Convert<R>(RawValue);
            }
            throw new InvalidCastException($"Could not return value of type {typeof(T).Name} as {typeof(R).Name}");
        }

        public override bool TryValueAs<R>(out R result)
        {
            if (typeof(T) == typeof(R))
            {
                result = (R)(object)RawValue;
                return true;
            }
            else
            {
                return Converter.Default.TryConvert(RawValue, out result);
            }
        }

        public override bool ValueEquals<R>(R value)
        {
            return TryValueAs(out R internalValue) && internalValue.Equals(value);
        }
    }
}
