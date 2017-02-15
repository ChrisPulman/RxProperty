using Windows.UI.Xaml;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Reactive.Bindings.Interactivity.TriggerBase"/>
    public class TriggerBase<T> : TriggerBase
        where T : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerBase{T}"/> class.
        /// </summary>
        public TriggerBase() : base(typeof(T))
        {
        }

#pragma warning disable IDE0009 // Member access should be qualified.

        /// <summary>
        /// Gets the associated object.
        /// </summary>
        /// <value>The associated object.</value>
        public new T AssociatedObject => base.AssociatedObject as T;

#pragma warning restore IDE0009 // Member access should be qualified.
    }
}