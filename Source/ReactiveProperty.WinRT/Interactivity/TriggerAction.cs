using Windows.UI.Xaml;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Reactive.Bindings.Interactivity.TriggerAction"/>
    public abstract class TriggerAction<T> : TriggerAction
        where T : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerAction{T}"/> class.
        /// </summary>
        public TriggerAction() : base(typeof(T))
        {
        }

#pragma warning disable IDE0009 // Member access should be qualified.

        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso
        /// cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> is attached.
        /// </summary>
        public new T AssociatedObject => base.AssociatedObject as T;

#pragma warning restore IDE0009 // Member access should be qualified.
    }
}