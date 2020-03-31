using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable IDE0058 // Expression value is never used

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// ServiceProviderDictionary.
    /// Implements the <see cref="IServiceProviderDictionary" />
    /// </summary>
    /// <seealso cref="IServiceProviderDictionary" />
    public class ServiceProviderDictionary : IServiceProviderDictionary
    {
        private readonly IDictionary<object, object?> _values;
        private readonly HashSet<Type> _servicesTypes = new HashSet<Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderDictionary" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public ServiceProviderDictionary(IDictionary<object, object?> values)
            => _values = values ?? new Dictionary<object, object?>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderDictionary" /> class.
        /// </summary>
        public ServiceProviderDictionary() => _values = new Dictionary<object, object?>();

        /// <summary>
        /// Gets or sets the <see cref="object" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.Object.</returns>
        public object? this[object key]
        {
            get => _values.TryGetValue(key, out var b) ? b : null;
            set
            {
                _values[key] = value;
                if (key is Type t)
                {
                    _servicesTypes.Add(t);
                }
            }
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public ICollection<object> Keys => _values.Keys;

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public ICollection<object?> Values => _values.Values;

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count => _values.Count;

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(object key, object? value)
        {
            _values.Add(key, value);
            if (key is Type t)
            {
                _servicesTypes.Add(t);
            }
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(KeyValuePair<object, object?> item)
        {
            _values.Add(item.Key, item.Value);
            if (item.Key is Type t)
            {
                _servicesTypes.Add(t);
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            _servicesTypes.Clear();
            _values.Clear();
        }

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.</returns>
        public bool Contains(KeyValuePair<object, object?> item) => _values.Contains(item);

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the specified key contains key; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(object key) => _values.ContainsKey(key);

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="index">Index of the array.</param>
        public void CopyTo(KeyValuePair<object, object?>[] array, int index) => _values.CopyTo(array, index);

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator{KeyValuePair{System.Object, System.Object}}.</returns>
        public IEnumerator<KeyValuePair<object, object?>> GetEnumerator() => _values.GetEnumerator();

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Remove(object key)
        {
            if (key is Type t)
            {
                _servicesTypes.Remove(t);
            }

            return _values.Remove(key);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Remove(KeyValuePair<object, object?> item)
        {
            if (item.Key is Type t)
            {
                _servicesTypes.Remove(t);
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            return _values.Remove(item);
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TryGetValue(object key, out object? value) => _values.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>System.Object.</returns>
        public object? GetService(Type serviceType) => _values.TryGetValue(serviceType, out var v) ? v : null;
    }
}