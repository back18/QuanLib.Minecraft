using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuanLib.Minecraft.Snbt.Tags
{
    public class NbtArray : NbtTag, IList<NbtTag>
    {
        public enum ArrayType
        {
            None, Byte, Integer, Long
        }

        private List<NbtTag> Values { get; } = new List<NbtTag>();
        private ArrayType Type { get; }

        public int Count => Values.Count;

        public bool IsReadOnly => false;

        public NbtTag this[int index] { get => Values[index]; set => Values[index] = value; }

        public NbtArray(ArrayType type = ArrayType.None)
        {
            Type = type;
        }

        /// <summary>
        /// Adds another item to the list.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <exception cref="InvalidOperationException">Thrown if item does not match other items in the list.</exception>
        public void Add(NbtTag item)
        {
            if (CanContain(item.GetType()))
            {
                Values.Add(item);
            }
            else
            {
                throw new InvalidOperationException($"Could not add item of type {item.GetType().FullName} to list of type {GetTypeOfValues()?.FullName}.");
            }
        }

        /// <summary>
        /// Inserts an item into the list.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        /// <exception cref="InvalidOperationException">Thrown if item does not match other items in the list.</exception>
        public void Insert(int index, NbtTag item)
        {
            if (CanContain(item.GetType()))
            {
                Values.Insert(index, item);
            }
            else
            {
                throw new InvalidOperationException($"Could not add item of type {item.GetType().FullName} to list of type {GetTypeOfValues()?.FullName}.");
            }
        }

        /// <summary>
        /// Removes item from the list.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was found and removed.</returns>
        public bool Remove(NbtTag item)
        {
            return CanContain(item.GetType()) ? Values.Remove(item) : false;
        }

        /// <summary>
        /// Removes the item at a certain index.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is not found in the array.</exception>
        public void RemoveAt(int index)
        {
            Values.RemoveAt(index);
        }

        /// <summary>
        /// Clears the array.
        /// </summary>
        public void Clear()
        {
            Values.Clear();
        }

        /// <summary>
        /// Checks if the array contains an nbt tag.
        /// </summary>
        /// <param name="item">The tag to check for.</param>
        /// <returns>Returns true if array contains the specified tag.</returns>
        public bool ContainsItem(NbtTag item)
        {
            return CanContain(item.GetType()) ? Values.Contains(item) : false;
        }

        /// <summary>
        /// Returns the index of the specified nbt tag.
        /// </summary>
        /// <param name="item">The tag to check for.</param>
        /// <returns>Returns the index of the specified item or -1 if not found.</returns>
        public int IndexOfItem(NbtTag item)
        {
            return CanContain(item.GetType()) ? Values.IndexOf(item) : -1;
        }

        /// <summary>
        /// Checks if the array contains a <see cref="NbtPrimitive"/> with a value that after possible conversion matches the specified value.
        /// </summary>
        /// <typeparam name="V">The type of value to check for.</typeparam>
        /// <param name="value">The value to check for.</param>
        /// <returns>Returns true if the array contains a <see cref="NbtPrimitive"/> of type <typeparamref name="V"/></returns>
        public bool ContainsValue<V>(V value)
        {
            return ItemsAre(typeof(NbtPrimitive)) ? Values.Any(v => v.As<NbtPrimitive>().ValueEquals(value)) : false;
        }

        /// <summary>
        /// Returns the index of the first <see cref="NbtPrimitive"/> with a value that after possible conversion matches the specified value.
        /// </summary>
        /// <typeparam name="V">The type of value to check for.</typeparam>
        /// <param name="item">The value to check for.</param>
        /// <returns>Returns the index of the specified value or -1 if not found.</returns>
        public int IndexOfValue<V>(V value)
        {
            return ItemsAre(typeof(NbtPrimitive)) ? Values.FindIndex(v => v.As<NbtPrimitive>().ValueEquals(value)) : -1;
        }

        /// <summary>
        /// Checks whether the items that this array contains (or can contain) can be returned as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <returns>Returns true if the items that this array contains (or can contain) can be returned as <typeparamref name="T"/>.</returns>
        public bool ItemsAre<T>() where T : NbtTag
        {
            return ItemsAre(typeof(T));
        }

        private bool ItemsAre(Type type)
        {
            var currentType = GetTypeOfValues();
            return currentType == null || type.IsAssignableFrom(currentType);
        }

        private bool CanContain(Type type)
        {
            var currentType = GetTypeOfValues();
            return currentType == null || currentType.IsAssignableFrom(type);
        }

        private Type GetTypeOfValues()
        {
            if (Values.Count > 0)
            {
                return Values[0].GetType();
            }
            else if (Type == ArrayType.None)
            {
                return null;
            }
            else if (Type == ArrayType.Byte)
            {
                return typeof(NbtPrimitive<sbyte>);
            }
            else if (Type == ArrayType.Integer)
            {
                return typeof(NbtPrimitive<int>);
            }
            else if (Type == ArrayType.Long)
            {
                return typeof(NbtPrimitive<long>);
            }
            else
            {
                throw new InvalidOperationException("Could not define type for array.");
            }
        }

        /// <summary>
        /// Returns all items as <typeparamref name="R"/>. This method is expensive, try to reuse the result of this method as much as possible.
        /// </summary>
        /// <typeparam name="R">The type to return the items as.</typeparam>
        /// <returns>Returns all items as <typeparamref name="R"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown if one of the items cannot be returned as <typeparamref name="R"/></exception>
        public IList<R> ItemsAs<R>() where R : NbtTag
        {
            return Values.Select(v => v.As<R>()).ToList();
        }

        /// <summary>
        /// Checks if the item are of type <typeparamref name="R"/> and if so, returns them. This method is expensive, try to reuse the result of this method as much as possible.
        /// </summary>
        /// <typeparam name="R">The type to return the items as.</typeparam>
        /// <param name="result">If possible, will contain all items as <typeparamref name="R"/></param>
        /// <returns>Returns true if the items could be returned as <typeparamref name="R"/></returns>
        public bool TryItemsAs<R>(out IList<R> result) where R : NbtTag
        {
            if (ItemsAre(typeof(R)))
            {
                result = ItemsAs<R>();
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Retrieves all items as <see cref="NbtPrimitive"/> and returns their values as <typeparamref name="R"/>. This method is expensive, try to reuse the result of this method as much as possible.
        /// </summary>
        /// <typeparam name="R">The type to return the value of the items as.</typeparam>
        /// <returns>Returns all values as <typeparamref name="R"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown if one of the items is not <see cref="NbtPrimitive"/> or the value cannot be returned as <typeparamref name="R"/></exception>
        public IList<R> ValuesAs<R>()
        {
            return Values.Select(v =>
            {
                var primitive = v.As<NbtPrimitive>();
                return primitive.ValueAs<R>();
            }).ToList();
        }

        /// <summary>
        /// Checks if the items are of type <see cref="NbtPrimitive"/> and the values inside can be returned as <typeparamref name="R"/> and if so, returns it. This method is expensive, try to reuse the result of this method as much as possible.
        /// </summary>
        /// <typeparam name="R">The type to return the value as.</typeparam>
        /// <param name="result">If possible, will contain the values of the the items as <typeparamref name="R"/></param>
        /// <returns>Returns true if the items are of type <see cref="NbtPrimitive"/> and the values inside can be returned as <typeparamref name="R"/></returns>
        public bool TryValuesAs<R>(out IList<R> result)
        {
            result = default;

            if (TryItemsAs<NbtPrimitive>(out var primitives))
            {
                var resultTmp = new List<R>();
                foreach (var primitive in primitives)
                {
                    if (primitive.TryValueAs<R>(out var value))
                    {
                        resultTmp.Add(value);
                    }
                    else return false;
                }
                result = resultTmp;
                return true;
            }
            else return false;
        }

        public int IndexOf(NbtTag item)
        {
            return Values.IndexOf(item);
        }

        public bool Contains(NbtTag item)
        {
            return Values.Contains(item);
        }

        public void CopyTo(NbtTag[] array, int arrayIndex)
        {
            Values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<NbtTag> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Values).GetEnumerator();
        }
    }
}
