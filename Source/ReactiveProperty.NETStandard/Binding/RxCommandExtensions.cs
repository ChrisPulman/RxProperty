using System;

namespace Reactive.Bindings.Binding
{
    /// <summary>
    /// ReactiveCommand bind to EventHandler
    /// </summary>
    public static class RxCommandExtensions
    {
        public static EventHandler ToEventHandler(this RxCommand self)
        {
            EventHandler h = (s, e) =>
            {
                if (self.CanExecute())
                {
                    self.Execute();
                }
            };
            return h;
        }

        public static EventHandler<TEventArgs> ToEventHandler<TEventArgs>(this RxCommand self)
        {
            EventHandler<TEventArgs> h = (s, e) =>
            {
                if (self.CanExecute())
                {
                    self.Execute();
                }
            };
            return h;
        }

        public static EventHandler<TEventArgs> ToEventHandler<TEventArgs, T>(this RxCommand<T> self, Func<TEventArgs, T> converter)
        {
            EventHandler<TEventArgs> h = (s, e) =>
            {
                var parameter = converter(e);
                if (self.CanExecute())
                {
                    self.Execute(parameter);
                }
            };
            return h;
        }
    }
}
