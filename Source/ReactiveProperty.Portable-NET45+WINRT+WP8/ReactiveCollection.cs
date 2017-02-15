using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Reactive.Bindings
{
    public static partial class ReactiveCollectionObservableExtensions
    {
        /// <summary>
        /// Source sequence as ObservableCollection. Operate scheduler is ReactivePropertyScheduler.
        /// </summary>
        public static ReactiveCollection<T> ToReactiveCollection<T>(this IObservable<T> source) =>
            new ReactiveCollection<T>(source);

        /// <summary>
        /// Source sequence as ObservableCollection. Operate scheduler is argument's scheduler.
        /// </summary>
        public static ReactiveCollection<T> ToReactiveCollection<T>(this IObservable<T> source, IScheduler scheduler) =>
            new ReactiveCollection<T>(source, scheduler);
    }

    /// <summary>
    /// ObservableCollection that operate on scheduler use by ***OnScheduler methods.
    /// </summary>
    public class ReactiveCollection<T> : ObservableCollection<T>, IDisposable
    {
        private readonly IScheduler scheduler;
        private readonly IDisposable subscription;

        /// <summary>
        /// Operate scheduler is UIDispatcherScheduler.
        /// </summary>
        public ReactiveCollection()
            : this(ReactivePropertyScheduler.Default)
        { }

        /// <summary>
        /// Operate scheduler is argument's scheduler.
        /// </summary>
        public ReactiveCollection(IScheduler scheduler)
        {
            this.scheduler = scheduler;
            this.subscription = Disposable.Empty;
        }

        /// <summary>
        /// Source sequence as ObservableCollection. Operate scheduler is UIDispatcherScheduler.
        /// </summary>
        public ReactiveCollection(IObservable<T> source)
            : this(source, ReactivePropertyScheduler.Default)
        {
        }

        /// <summary>
        /// Source sequence as ObservableCollection. Operate scheduler is argument's scheduler.
        /// </summary>
        public ReactiveCollection(IObservable<T> source, IScheduler scheduler)
        {
            this.scheduler = scheduler;
            this.subscription = source.ObserveOn(scheduler).Subscribe(this.Add);
        }

        /// <summary>
        /// Add called on scheduler
        /// </summary>
        public void AddOnScheduler(T item) => this.scheduler.Schedule(() => Add(item));

        /// <summary>
        /// Add called on scheduler
        /// </summary>
        /// <param name="items"></param>
        public void AddRangeOnScheduler(params T[] items) => this.scheduler.Schedule(() => { foreach (T item in items) { Add(item); } });

        /// <summary>
        /// Add called on scheduler
        /// </summary>
        /// <param name="items"></param>
        public void AddRangeOnScheduler(IEnumerable<T> items) => this.AddRangeOnScheduler(items.ToArray());

        /// <summary>
        /// Clear called on scheduler
        /// </summary>
        public void ClearOnScheduler() => this.scheduler.Schedule(() => Clear());

        /// <summary>
        /// Unsubcribe source sequence.
        /// </summary>
        public void Dispose() => this.subscription.Dispose();

        /// <summary>
        /// Get(indexer get) called on scheduler, IObservable length is one.
        /// </summary>
        public IObservable<T> GetOnScheduler(int index) => Observable.Start(() => this[index], this.scheduler);

        /// <summary>
        /// Insert called on scheduler
        /// </summary>
        public void InsertOnScheduler(int index, T item) => this.scheduler.Schedule(() => Insert(index, item));

        /// <summary>
        /// Move called on scheduler
        /// </summary>
        public void MoveOnScheduler(int oldIndex, int newIndex) => this.scheduler.Schedule(() => Move(oldIndex, newIndex));

        /// <summary>
        /// RemoveAt called on scheduler
        /// </summary>
        public void RemoveAtOnScheduler(int index) => this.scheduler.Schedule(() => RemoveAt(index));

        /// <summary>
        /// Remove called on scheduler
        /// </summary>
        public void RemoveOnScheduler(T item) => this.scheduler.Schedule(() => Remove(item));

        /// <summary>
        /// Set(indexer set) called on scheduler
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void SetOnScheduler(int index, T value) => this.scheduler.Schedule(() => this[index] = value);
    }
}