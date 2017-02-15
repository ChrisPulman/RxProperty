using System;
using System.Reactive;

#if NETFX_CORE
using Windows.UI.Xaml;
#else

using System.Windows;
using System.Windows.Interactivity;

#endif

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// ///
    /// </summary>
    /// <seealso cref="System.Windows.Interactivity.TriggerAction{System.Windows.DependencyObject}"/>
    [Obsolete("Please use EventToReactiveProperty")]
    public class EventToReactive : TriggerAction<DependencyObject>
    {
        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to the action. If the action does not require a parameter, the parameter
        /// may be set to a null reference.
        /// </param>
        protected override void Invoke(object parameter)
        {
            if (this.ReactiveProperty == null)
            {
                return;
            }

            if (this.IgnoreEventArgs)
            {
                ((IReactiveProperty)this.ReactiveProperty).Value = new Unit();
            }
            else
            {
                var converter = this.Converter;
                ((IReactiveProperty)this.ReactiveProperty).Value =
                    (converter != null) ? converter(parameter) : parameter;
            }
        }

        // default is false
        /// <summary>
        /// Gets or sets a value indicating whether [ignore event arguments].
        /// </summary>
        /// <value><c>true</c> if [ignore event arguments]; otherwise, <c>false</c>.</value>
        public bool IgnoreEventArgs { get; set; }

        /// <summary>
        /// Gets or sets the reactive property.
        /// </summary>
        /// <value>The reactive property.</value>
        public object ReactiveProperty
        {
            get => GetValue(ReactivePropertyProperty); set => SetValue(ReactivePropertyProperty, value);
        }

        /// <summary>
        /// The reactive property property
        /// </summary>
        public static readonly DependencyProperty ReactivePropertyProperty =
            DependencyProperty.Register("ReactiveProperty", typeof(IReactiveProperty), typeof(EventToReactive), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the converter.
        /// </summary>
        /// <value>The converter.</value>
        public Func<object, object> Converter
        {
            get => (Func<object, object>)GetValue(ConverterProperty); set => SetValue(ConverterProperty, value);
        }

        /// <summary>
        /// The converter property
        /// </summary>
        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register("Converter", typeof(Func<object, object>), typeof(EventToReactive), new PropertyMetadata(null));
    }
}