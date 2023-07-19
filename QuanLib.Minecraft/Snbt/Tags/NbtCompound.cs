using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace QuanLib.Minecraft.Snbt.Tags
{
    public class NbtCompound : NbtTag, IDictionary<string, NbtTag>
    {
        private Dictionary<string, NbtTag> Items { get; } = new Dictionary<string, NbtTag>();

        /// <summary>
        /// Gets all keys this Compound contains
        /// </summary>
        public ICollection<string> Keys => Items.Keys;

        public ICollection<NbtTag> Values => Items.Values;

        /// <summary>
        /// Gets the amount of entries in this compound
        /// </summary>
        public int Count => Items.Count;

        public bool IsReadOnly => false;

        public NbtTag this[string key] { get => Items[key]; set => Items[key] = value; }

        /// <summary>
        /// Adds entry to compound.
        /// </summary>
        /// <param name="key">String key.</param>
        /// <param name="tag">The nbt tag.</param>
        /// <exception cref="ArgumentException">Thrown if key already exists in compound.</exception>
        public void Add(string key, NbtTag tag)
        {
            Items.Add(key, tag);
        }

        /// <summary>
        /// Clears compound.
        /// </summary>
        public void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// Checks if the compound contains specified key.
        /// </summary>
        /// <param name="key">The key to check for.</param>
        /// <returns>True if the compound contains specified key.</returns>
        public bool ContainsKey(string key)
        {
            return Items.ContainsKey(key);
        }

        /// <summary>
        /// Remove a key from the compound.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>Returns true if the key was found and removed.</returns>
        public bool Remove(string key)
        {
            return Items.Remove(key);
        }

        /// <summary>
        /// Checks if the item at the specified key is <typeparamref name="R"/>
        /// </summary>
        /// <typeparam name="R">The tag type to check.</typeparam>
        /// <param name="key">The key associated with the item.</param>
        /// <returns>Returns true if the item associated with the key is <typeparamref name="R"/></returns>
        /// <exception cref="KeyNotFoundException">Thrown if the dictionary does not contain the key.</exception>
        public bool ItemIs<R>(string key) where R : NbtTag
        {
            return Items[key].Is<R>();
        }

        /// <summary>
        /// Returns the item at the specified key as <typeparamref name="R"/>
        /// </summary>
        /// <typeparam name="R">The tag type to return.</typeparam>
        /// <param name="key">The key associated with the item.</param>
        /// <returns>Returns the item associated with the key as <typeparamref name="R"/></returns>
        /// <exception cref="KeyNotFoundException">Thrown if the dictionary does not contain the key.</exception>
        /// <exception cref="InvalidCastException">Thrown if the item cannot be returned as <typeparamref name="R"/></exception>
        public R ItemAs<R>(string key) where R : NbtTag
        {
            return Items[key].As<R>();
        }

        /// <summary>
        /// Returns true if the item associated with the key exists and can be returned as <typeparamref name="R"/> and if so, the value.
        /// </summary>
        /// <typeparam name="R">The type of the item.</typeparam>
        /// <param name="key">The key associated with the item.</param>
        /// <param name="value">Set if the item is found and able to be returned as <typeparamref name="R"/>.</param>
        /// <param name="requireKey">If true, an exception will be thrown when the key is not found.</param>
        /// <param name="requireType">If true, an exception will be thrown when the value can not be returned as <typeparamref name="R"/>.</param>
        /// <returns>Returns true if the item associated with the key exists and can be returned as <typeparamref name="R"/></returns>
        /// <exception cref="KeyNotFoundException">Thrown if the key is required and the dictionary does not contain the key.</exception>
        /// <exception cref="InvalidCastException">Thrown if the type conversion is required and the item cannot be returned as <typeparamref name="R"/></exception>
        public bool TryItemAs<R>(string key, out R value, bool requireKey = false, bool requireType = false) where R : NbtTag
        {
            value = default;
            var hasValue = Items.TryGetValue(key, out var tag);
            var hasType = hasValue ? tag.TryAs(out value) : false;
            if (!hasValue && requireKey)
            {
                throw new KeyNotFoundException($"Key {key} not found in compound.");
            }
            else if (!hasType && requireType)
            {
                throw new InvalidCastException($"Value in compound can not be returned as {typeof(R).FullName}");
            }
            else if (hasValue && hasType)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the value of the <see cref="NbtPrimitive"/> at the specified key as <typeparamref name="R"/>
        /// </summary>
        /// <typeparam name="R">The value type to return.</typeparam>
        /// <param name="key">The key associated with the value.</param>
        /// <returns>Returns the value of the <see cref="NbtPrimitive"/> associated with the key as <typeparamref name="R"/></returns>
        /// <exception cref="KeyNotFoundException">Thrown if the dictionary does not contain the key.</exception>
        /// <exception cref="InvalidCastException">Thrown if the item is not <see cref="NbtPrimitive"/> or the value inside cannot be converted to <typeparamref name="R"/>.</exception>
        public R ValueAs<R>(string key)
        {
            return Items[key].As<NbtPrimitive>().ValueAs<R>();
        }

        /// <summary>
        /// Returns true if a <see cref="NbtPrimitive"/> associated with the key exists and the value can be returned as <typeparamref name="R"/> and if so, the value.
        /// </summary>
        /// <typeparam name="R">The type of the value.</typeparam>
        /// <param name="key">The key associated with the item.</param>
        /// <param name="value">Set if the <see cref="NbtPrimitive"/> is found and the value is able to be returned as <typeparamref name="R"/>.</param>
        /// <param name="requireKey">If true, an exception will be thrown when the key is not found.</param>
        /// <param name="requireType">If true, an exception will be thrown when the item is not <see cref="NbtPrimitive"/> or the value inside can not be returned as <typeparamref name="R"/>.</param>
        /// <returns>Returns true if a <see cref="NbtPrimitive"/> associated with the key exists and the value can be returned as <typeparamref name="R"/></returns>
        /// <exception cref="KeyNotFoundException">Thrown if the key is required and the dictionary does not contain the key.</exception>
        /// <exception cref="InvalidCastException">Thrown if the type conversion is required and the item cannot be returned as <typeparamref name="R"/></exception>
        public bool TryValueAs<R>(string key, out R value, bool requireKey = false, bool requireType = false)
        {
            value = default;
            if (!TryItemAs<NbtPrimitive>(key, out var primitive, requireKey, requireType))
            {
                return false;
            }

            var canConvert = primitive.TryValueAs(out value);
            if (!canConvert && requireType)
            {
                throw new InvalidCastException($"Value in compound can not be returned as {typeof(R).FullName}");
            }
            else if (canConvert)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns all items as <typeparamref name="R"/>. This method is expensive, try to reuse the result of this method as much as possible.
        /// </summary>
        /// <typeparam name="R">The type to return the items as.</typeparam>
        /// <returns>Returns all items as <typeparamref name="R"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown if one of the items cannot be returned as <typeparamref name="R"/></exception>
        public IEnumerable<KeyValuePair<string, R>> ItemsAs<R>() where R : NbtTag
        {
            return Items.Select(i => new KeyValuePair<string, R>(i.Key, i.Value.As<R>())).ToList();
        }

        /// <summary>
        /// Retrieves all items as <see cref="NbtPrimitive"/> and returns their values as <typeparamref name="R"/>. This method is expensive, try to reuse the result of this method as much as possible.
        /// </summary>
        /// <typeparam name="R">The type to return the value of the items as.</typeparam>
        /// <returns>Returns all values as <typeparamref name="R"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown if one of the items is not <see cref="NbtPrimitive"/> or the value cannot be returned as <typeparamref name="R"/></exception>
        public IEnumerable<KeyValuePair<string, R>> ValuesAs<R>()
        {
            return Items.Select(i => new KeyValuePair<string, R>(i.Key, i.Value.As<NbtPrimitive>().ValueAs<R>())).ToList();
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out NbtTag value)
        {
            return Items.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, NbtTag>> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }

        void ICollection<KeyValuePair<string, NbtTag>>.Add(KeyValuePair<string, NbtTag> item)
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<string, NbtTag>>.Contains(KeyValuePair<string, NbtTag> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string, NbtTag>>.CopyTo(KeyValuePair<string, NbtTag>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<string, NbtTag>>.Remove(KeyValuePair<string, NbtTag> item)
        {
            throw new NotImplementedException();
        }
    }
}
