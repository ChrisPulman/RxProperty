using System;
using Windows.UI.Xaml;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// ///
    /// </summary>
    /// <seealso cref="Reactive.Bindings.Interactivity.TriggerAction"/>
    public abstract class TargetedTriggerAction : TriggerAction
    {
        /// <summary>
        /// The target object property
        /// </summary>
        public static readonly DependencyProperty TargetObjectProperty =
            DependencyProperty.Register(
                "TargetObject",
                typeof(object),
                typeof(TargetedTriggerAction),
                new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetedTriggerAction"/> class.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        public TargetedTriggerAction(Type targetType) : base(typeof(DependencyObject)) => this.TargetType = targetType;

        /// <summary>
        /// Gets or sets the target object.
        /// </summary>
        /// <value>The target object.</value>
        public object TargetObject
        {
            get => (object)GetValue(TargetObjectProperty); set => SetValue(TargetObjectProperty, value);
        }

        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        /// <value>The type of the target.</value>
        public Type TargetType { get; private set; }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        protected object Target
        {
            get
            {
                return this.TargetObject ?? this.AssociatedObject;
            }
        }
    }
}