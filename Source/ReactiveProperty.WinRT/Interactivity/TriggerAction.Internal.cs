using System;
using System.Reactive;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// ///
    /// </summary>
    /// <seealso cref="Reactive.Bindings.Interactivity.Behavior"/>
    /// <seealso cref="Microsoft.Xaml.Interactivity.IAction"/>
    public abstract class TriggerAction : Behavior, IAction
    {
        /// <summary>
        /// The is enabled property
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(
                "IsEnabled",
                typeof(bool),
                typeof(TriggerAction),
                new PropertyMetadata(true));

        internal TriggerAction(Type associatedType) : base(associatedType)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty); set => SetValue(IsEnabledProperty, value);
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="T:System.Object"/> that is passed to the action by the behavior. Generally
        /// this is <seealso cref="P:Microsoft.Xaml.Interactivity.IBehavior.AssociatedObject"/> or a
        /// target object.
        /// </param>
        /// <param name="parameter">The value of this parameter is determined by the caller.</param>
        /// <returns>Returns the result of the action.</returns>
        /// <remarks>
        /// An example of parameter usage is EventTriggerBehavior, which passes the EventArgs as a
        /// parameter to its actions.
        /// </remarks>
        public object Execute(object sender, object parameter)
        {
            if (!this.IsEnabled)
            {
                return null;
            }

            this.Invoke(parameter);
            return Unit.Default;
        }

        /// <summary>
        /// Invokes the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected abstract void Invoke(object parameter);
    }
}