namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Reactive.Bindings.Interactivity.TargetedTriggerAction"/>
    public abstract class TargetedTriggerAction<T> : TargetedTriggerAction
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetedTriggerAction{T}"/> class.
        /// </summary>
        public TargetedTriggerAction() : base(typeof(T))
        {
        }

#pragma warning disable IDE0009 // Member access should be qualified.

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        protected new T Target => base.Target as T;

#pragma warning restore IDE0009 // Member access should be qualified.
    }
}