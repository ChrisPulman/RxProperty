using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Windows.UI.Xaml;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// ///
    /// </summary>
    public enum Op
    {
        /// <summary>
        /// =
        /// </summary>
        Eq,

        /// <summary>
        /// &gt;=
        /// </summary>
        GtEq,

        /// <summary>
        /// &gt;
        /// </summary>
        Gt,

        /// <summary>
        /// &lt;=
        /// </summary>
        LtEq,

        /// <summary>
        /// &lt;
        /// </summary>
        Lt
    }

    /// <summary>
    /// ///
    /// </summary>
    /// <seealso cref="Reactive.Bindings.Interactivity.Behavior{Reactive.Bindings.Interactivity.TriggerBase}"/>
    public class ConditionBehavior : Behavior<TriggerBase>
    {
        /// <summary>
        /// The compare operator property
        /// </summary>
        public static readonly DependencyProperty CompareOperatorProperty =
            DependencyProperty.Register("CompareOperator", typeof(Op), typeof(ConditionBehavior), new PropertyMetadata(Op.Eq));

        /// <summary>
        /// The LHS property
        /// </summary>
        public static readonly DependencyProperty LhsProperty =
            DependencyProperty.Register("Lhs", typeof(object), typeof(ConditionBehavior), new PropertyMetadata(null));

        /// <summary>
        /// The RHS property
        /// </summary>
        public static readonly DependencyProperty RhsProperty =
            DependencyProperty.Register("Rhs", typeof(object), typeof(ConditionBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the compare operator.
        /// </summary>
        /// <value>The compare operator.</value>
        public Op CompareOperator
        {
            get => (Op)GetValue(CompareOperatorProperty); set => SetValue(CompareOperatorProperty, value);
        }

        /// <summary>
        /// Gets or sets the LHS.
        /// </summary>
        /// <value>The LHS.</value>
        public object Lhs
        {
            get => (object)GetValue(LhsProperty); set => SetValue(LhsProperty, value);
        }

        /// <summary>
        /// Gets or sets the RHS.
        /// </summary>
        /// <value>The RHS.</value>
        public object Rhs
        {
            get => (object)GetValue(RhsProperty); set => SetValue(RhsProperty, value);
        }

        /// <summary>
        /// Called when [attached].
        /// </summary>
        protected override void OnAttached() => this.AssociatedObject.PreviewInvoke += this.AssociatedObjectPreviewInvoke;

        /// <summary>
        /// Called when [detaching].
        /// </summary>
        protected override void OnDetaching() => this.AssociatedObject.PreviewInvoke -= this.AssociatedObjectPreviewInvoke;

        private void AssociatedObjectPreviewInvoke(object sender, PreviewInvokeEventArgs e)
        {
            try
            {
                Type comparerType = typeof(Comparer<>).MakeGenericType(this.Lhs.GetType());
                PropertyInfo defaultProperty = comparerType.GetTypeInfo().GetDeclaredProperty("Default");
                var comparer = (IComparer)defaultProperty.GetValue(null);
                var comparerResult = comparer.Compare(this.Lhs, System.Convert.ChangeType(this.Rhs, comparerType));
                switch (this.CompareOperator)
                {
                    case Op.Eq:
                        e.Cancelling = !(comparerResult == 0);
                        break;

                    case Op.Gt:
                        e.Cancelling = !(comparerResult > 0);
                        break;

                    case Op.GtEq:
                        e.Cancelling = !(comparerResult >= 0);
                        break;

                    case Op.Lt:
                        e.Cancelling = !(comparerResult < 0);
                        break;

                    case Op.LtEq:
                        e.Cancelling = !(comparerResult <= 0);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                e.Cancelling = true;
            }
        }
    }
}