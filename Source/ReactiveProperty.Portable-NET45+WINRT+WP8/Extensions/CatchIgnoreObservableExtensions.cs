using System;
using System.Reactive.Linq;

namespace Reactive.Bindings.Extensions
{
    /// <summary>
    /// Catch Ignore ObservableExtensions
    /// </summary>
    public static class CatchIgnoreObservableExtensions
    {
        /// <summary>
        /// Catch exception and return Observable.Empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IObservable<TSource> CatchIgnore<TSource>(
            this IObservable<TSource> source) =>
            source.Catch(Observable.Empty<TSource>());

        /// <summary>
        /// Catch exception and return Observable.Empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="errorAction">The error action.</param>
        /// <returns></returns>
        public static IObservable<TSource> CatchIgnore<TSource, TException>(
            this IObservable<TSource> source, Action<TException> errorAction)
            where TException : Exception => source.Catch((TException ex) =>
                                                      {
                                                          errorAction(ex);
                                                          return Observable.Empty<TSource>();
                                                      });
    }
}