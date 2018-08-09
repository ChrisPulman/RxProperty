using System;

namespace Reactive.Bindings.Binding
{
    /// <summary>
    /// ReactiveCommand bind to EventHandler
    /// </summary>
    public static class RxCommandExtensions
    {
        /// <summary>
        /// To the event handler.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns></returns>
        public static EventHandler ToEventHandler(this RxCommand self)
        {
            return (s, e) => {
                if (self.CanExecute()) {
                    self.Execute();
                }
            };
        }

        /// <summary>
        /// To the event handler.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="self">The self.</param>
        /// <returns></returns>
        public static EventHandler<TEventArgs> ToEventHandler<TEventArgs>(this RxCommand self)
        {
            return (s, e) => {
                if (self.CanExecute()) {
                    self.Execute();
                }
            };
        }

        /// <summary>
        /// To the event handler.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">The self.</param>
        /// <param name="converter">The converter.</param>
        /// <returns></returns>
        public static EventHandler<TEventArgs> ToEventHandler<TEventArgs, T>(this RxCommand<T> self, Func<TEventArgs, T> converter)
        {
            return (s, e) => {
                var parameter = converter(e);
                if (self.CanExecute()) {
                    self.Execute(parameter);
                }
            };
        }
    }
}
