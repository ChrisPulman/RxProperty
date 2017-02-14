using Windows.UI.Xaml;

namespace Reactive.Bindings.Interactivity
{
    public abstract class TriggerAction<T> : TriggerAction
        where T : DependencyObject
    {
        public TriggerAction() : base(typeof(T))
        {
        }

        public new T AssociatedObject => base.AssociatedObject as T;
    }
}