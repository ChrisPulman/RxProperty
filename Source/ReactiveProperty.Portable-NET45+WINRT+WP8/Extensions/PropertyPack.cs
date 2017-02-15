using System;
using System.Reflection;

namespace Reactive.Bindings.Extensions
{
    /// <summary>
    /// Represents property and instance package.
    /// </summary>
    /// <typeparam name="TInstance">Type of instance</typeparam>
    /// <typeparam name="TValue">Type of property value</typeparam>
    public class PropertyPack<TInstance, TValue>
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="instance">Target instance</param>
        /// <param name="property">Target property info</param>
        /// <param name="value">Property value</param>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/>"/&gt; is <c>null</c>.</exception>
        internal PropertyPack(TInstance instance, PropertyInfo property, TValue value)
        {
            if (instance == null)
            {
#pragma warning disable IDE0016 // Use 'throw' expression
                throw new ArgumentNullException(nameof(instance));
#pragma warning restore IDE0016 // Use 'throw' expression
            }

            this.Instance = instance;
            this.Property = property ?? throw new ArgumentNullException(nameof(property));
            this.Value = value;
        }

        /// <summary>
        /// Gets instance which has property.
        /// </summary>
        public TInstance Instance { get; }

        /// <summary>
        /// Gets target property info.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Gets target property value.
        /// </summary>
        public TValue Value { get; }
    }

    /// <summary>
    /// Provides PropertyPack static members.
    /// </summary>
    internal static class PropertyPack
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="instance">Target instance</param>
        /// <param name="property">Target property info</param>
        /// <param name="value">Property value</param>
        /// <returns>Created instance</returns>
        public static PropertyPack<TInstance, TValue> Create<TInstance, TValue>(TInstance instance, PropertyInfo property, TValue value) =>
            new PropertyPack<TInstance, TValue>(instance, property, value);
    }
}