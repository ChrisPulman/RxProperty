using System;
using System.Reflection;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// ///
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.DependencyObject"/>
    /// <seealso cref="Microsoft.Xaml.Interactivity.IBehavior"/>
    public abstract class Behavior : DependencyObject, IBehavior
    {
        /// <summary>
        /// The associated object changed
        /// </summary>
        public EventHandler AssociatedObjectChanged;

        internal Behavior(Type associatedType) => this.AssociatedType = associatedType;

        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso
        /// cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> is attached.
        /// </summary>
        public DependencyObject AssociatedObject { get; private set; }

        /// <summary>
        /// Gets the type of the associated.
        /// </summary>
        /// <value>The type of the associated.</value>
        public Type AssociatedType { get; private set; }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">
        /// The <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso
        /// cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> will be attached.
        /// </param>
        public void Attach(DependencyObject associatedObject)
        {
            if (this.AssociatedObject == associatedObject)
            {
                return;
            }

            this.AssertAttachArgument(associatedObject);

            this.AssociatedObject = associatedObject;
            this.OnAssociatedObjectChanged();

            this.OnAttached();
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            this.OnDetaching();
            this.AssociatedObject = null;
            this.OnAssociatedObjectChanged();
        }

        /// <summary>
        /// Called when [attached].
        /// </summary>
        protected virtual void OnAttached()
        {
        }

        /// <summary>
        /// Called when [detaching].
        /// </summary>
        protected virtual void OnDetaching()
        {
        }

        private void AssertAttachArgument(DependencyObject associatedObject)
        {
            if (this.AssociatedObject != null)
            {
                throw new InvalidOperationException("multiple time associate.");
            }

            if (associatedObject == null)
            {
                throw new ArgumentNullException("associatedObject");
            }

            if (!this.AssociatedType.GetTypeInfo().IsAssignableFrom(associatedObject.GetType().GetTypeInfo()))
            {
                throw new ArgumentException(string.Format("{0} can't assign {1}",
                    associatedObject.GetType().FullName,
                    this.AssociatedType.FullName));
            }
        }

        private void OnAssociatedObjectChanged() => this.AssociatedObjectChanged?.Invoke(this, EventArgs.Empty);
    }
}