using Windows.UI.Xaml;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Reactive.Bindings.Interactivity.Behavior"/>
    public abstract class Behavior<T> : Behavior
        where T : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Behavior{T}"/> class.
        /// </summary>
        protected Behavior() : base(typeof(T))
        {
        }

        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso
        /// cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> is attached.
        /// </summary>
#pragma warning disable IDE0009 // Member access should be qualified.

        protected new T AssociatedObject => base.AssociatedObject as T;

#pragma warning restore IDE0009 // Member access should be qualified.
    }
}