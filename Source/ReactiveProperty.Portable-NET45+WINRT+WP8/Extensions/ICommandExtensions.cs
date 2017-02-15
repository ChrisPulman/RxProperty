using System;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Reactive.Bindings.Extensions
{
    /// <summary>
    /// ///
    /// </summary>
    public static class ICommandExtensions
    {
        /// <summary>
        /// Converts CanExecuteChanged to an observable sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IObservable<EventArgs> CanExecuteChangedAsObservable<T>(this T source)
            where T : ICommand =>
            Observable.FromEvent<EventHandler, EventArgs>(
                h => (sender, e) => h(e),
                h => source.CanExecuteChanged += h,
                h => source.CanExecuteChanged -= h);
    }
}