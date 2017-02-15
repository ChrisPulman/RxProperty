using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Reactive.Bindings.Extensions
{
    /// <summary>
    /// ///
    /// </summary>
    public static class RetryObservableExtensions
    {
        /// <summary>
        /// <para>Repeats the source observable sequence until it successfully terminates.</para>
        /// <para>This is same as Retry().</para>
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource>(
            this IObservable<TSource> source) =>
            source.Retry();

        /// <summary>
        /// When catched exception, do onError action and repeat observable sequence.
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError)
            where TException : Exception =>
            source.OnErrorRetry(onError, TimeSpan.Zero);

        /// <summary>
        /// When catched exception, do onError action and repeat observable sequence after delay time.
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError, TimeSpan delay)
            where TException : Exception =>
            source.OnErrorRetry(onError, int.MaxValue, delay);

        /// <summary>
        /// When catched exception, do onError action and repeat observable sequence during within retryCount.
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError, int retryCount)
            where TException : Exception =>
            source.OnErrorRetry(onError, retryCount, TimeSpan.Zero);

        /// <summary>
        /// When catched exception, do onError action and repeat observable sequence after delay time
        /// during within retryCount.
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError, int retryCount, TimeSpan delay)
            where TException : Exception =>
            source.OnErrorRetry(onError, retryCount, delay, Scheduler.Default);

        /// <summary>
        /// When catched exception, do onError action and repeat observable sequence after delay
        /// time(work on delayScheduler) during within retryCount.
        /// </summary>
        public static IObservable<TSource> OnErrorRetry<TSource, TException>(
            this IObservable<TSource> source, Action<TException> onError, int retryCount, TimeSpan delay, IScheduler delayScheduler)
            where TException : Exception =>
            Observable.Defer(() =>
                {
                    TimeSpan dueTime = (delay.Ticks < 0) ? TimeSpan.Zero : delay;
                    IObservable<TSource> empty = Observable.Empty<TSource>();
                    var count = 0;

#pragma warning disable RCS1127 // Merge local declaration with initialization.
                    IObservable<TSource> self = null;
#pragma warning restore RCS1127 // Merge local declaration with initialization.
                    self = source.Catch((TException ex) =>
                    {
                        onError(ex);

                        return (++count < retryCount)
                            ? (dueTime == TimeSpan.Zero)
                                ? self.SubscribeOn(Scheduler.CurrentThread)
                                : empty.Delay(dueTime, delayScheduler).Concat(self).SubscribeOn(Scheduler.CurrentThread)
                            : Observable.Throw<TSource>(ex);
                    });
                    return self;
                });
    }
}