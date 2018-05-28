using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace Reactive.Bindings
{
    /// <summary>
    /// ReactiveCommand factory extension methods.
    /// </summary>
    public static class RxCommandExtensions
    {
        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on UIDispatcherScheduler.
        /// </summary>
        public static RxCommand ToReactiveCommand(this IObservable<bool> canExecuteSource, bool initialValue = true) =>
            new RxCommand(canExecuteSource, initialValue);

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on scheduler.
        /// </summary>
        public static RxCommand ToReactiveCommand(this IObservable<bool> canExecuteSource, IScheduler scheduler, bool initialValue = true) =>
            new RxCommand(canExecuteSource, scheduler, initialValue);

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on UIDispatcherScheduler.
        /// </summary>
        public static RxCommand<T> ToReactiveCommand<T>(this IObservable<bool> canExecuteSource, bool initialValue = true) =>
            new RxCommand<T>(canExecuteSource, initialValue);

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on scheduler.
        /// </summary>
        public static RxCommand<T> ToReactiveCommand<T>(this IObservable<bool> canExecuteSource, IScheduler scheduler, bool initialValue = true) =>
            new RxCommand<T>(canExecuteSource, scheduler, initialValue);

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        /// <param name="self">ReactiveCommand</param>
        /// <param name="onNext">Action</param>
        /// <param name="postProcess">Handling of the subscription.</param>
        /// <returns>Same of self argument</returns>
        public static RxCommand WithSubscribe(this RxCommand self, Action onNext, Action<IDisposable> postProcess = null)
        {
            var d = self.Subscribe(onNext);
            postProcess?.Invoke(d);
            return self;
        }

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        /// <typeparam name="T">ReactiveCommand type argument.</typeparam>
        /// <param name="self">ReactiveCommand</param>
        /// <param name="onNext">Action</param>
        /// <param name="postProcess">Handling of the subscription.</param>
        /// <returns>Same of self argument</returns>
        public static RxCommand<T> WithSubscribe<T>(this RxCommand<T> self, Action<T> onNext, Action<IDisposable> postProcess = null)
        {
            var d = self.Subscribe(onNext);
            postProcess?.Invoke(d);
            return self;
        }

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        /// <param name="self">ReactiveCommand</param>
        /// <param name="onNext">Action</param>
        /// <param name="disposable">The return value of self.Subscribe(onNext)</param>
        /// <returns>Same of self argument</returns>
        public static RxCommand WithSubscribe(this RxCommand self, Action onNext, out IDisposable disposable)
        {
            disposable = self.Subscribe(onNext);
            return self;
        }

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        /// <typeparam name="T">ReactiveCommand type argument.</typeparam>
        /// <param name="self">ReactiveCommand</param>
        /// <param name="onNext">Action</param>
        /// <param name="disposable">The return value of self.Subscribe(onNext)</param>
        /// <returns>Same of self argument</returns>
        public static RxCommand<T> WithSubscribe<T>(this RxCommand<T> self, Action<T> onNext, out IDisposable disposable)
        {
            disposable = self.Subscribe(onNext);
            return self;
        }
    }

    /// <summary>
    /// ICommand and IObservable&lt;T&gt; implementation class.
    /// </summary>
    /// <typeparam name="T">Type of command argument.</typeparam>
    public class RxCommand<T> : IObservable<T>, ICommand, IDisposable
    {
        /// <summary>
        /// CanExecute is always true. When disposed CanExecute change false called on UIDispatcherScheduler.
        /// </summary>
        public RxCommand()
            : this(Observable.Never<bool>())
        {
        }

        /// <summary>
        /// CanExecute is always true. When disposed CanExecute change false called on scheduler.
        /// </summary>
        public RxCommand(IScheduler scheduler)
            : this(Observable.Never<bool>(), scheduler)
        {
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on ReactivePropertyScheduler.
        /// </summary>
        public RxCommand(IObservable<bool> canExecuteSource, bool initialValue = true)
            : this(canExecuteSource, ReactivePropertyScheduler.Default, initialValue)
        {
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on scheduler.
        /// </summary>
        public RxCommand(IObservable<bool> canExecuteSource, IScheduler scheduler, bool initialValue = true)
        {
            this.IsCanExecute = initialValue;
            this.Scheduler = scheduler;
            this.CanExecuteSubscription = canExecuteSource
                .DistinctUntilChanged()
                .ObserveOn(scheduler)
                .Subscribe(b => {
                    IsCanExecute = b;
                    this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                });
        }

        /// <summary>
        /// ICommand#CanExecuteChanged
        /// </summary>
        public event EventHandler CanExecuteChanged;

        private IDisposable CanExecuteSubscription { get; }

        private bool IsCanExecute { get; set; }

        private bool IsDisposed { get; set; } = false;

        private IScheduler Scheduler { get; }

        private Subject<T> Trigger { get; } = new Subject<T>();

        /// <summary>
        /// Return current canExecute status.
        /// </summary>
        public bool CanExecute() => IsCanExecute;

        /// <summary>
        /// Return current canExecute status. parameter is ignored.
        /// </summary>
        bool ICommand.CanExecute(object parameter) => IsCanExecute;

        /// <summary>
        /// Stop all subscription and lock CanExecute is false.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;

            IsDisposed = true;
            Trigger.OnCompleted();
            Trigger.Dispose();
            CanExecuteSubscription.Dispose();

            if (IsCanExecute) {
                IsCanExecute = false;
                Scheduler.Schedule(() => {
                    IsCanExecute = false;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                });
            }
        }

        /// <summary>
        /// Push parameter to subscribers.
        /// </summary>
        public void Execute(T parameter) => Trigger.OnNext(parameter);

        /// <summary>
        /// Push parameter to subscribers.
        /// </summary>
        void ICommand.Execute(object parameter) => Trigger.OnNext((T)parameter);

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        public IDisposable Subscribe(IObserver<T> observer) => Trigger.Subscribe(observer);
    }

    /// <summary>
    /// Represents ReactiveCommand&lt;object&gt;
    /// </summary>
    public class RxCommand : RxCommand<object>
    {
        /// <summary>
        /// CanExecute is always true. When disposed CanExecute change false called on UIDispatcherScheduler.
        /// </summary>
        public RxCommand()
            : base()
        { }

        /// <summary>
        /// CanExecute is always true. When disposed CanExecute change false called on scheduler.
        /// </summary>
        public RxCommand(IScheduler scheduler)
            : base(scheduler)
        {
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on UIDispatcherScheduler.
        /// </summary>
        public RxCommand(IObservable<bool> canExecuteSource, bool initialValue = true)
            : base(canExecuteSource, initialValue)
        {
        }

        /// <summary>
        /// CanExecuteChanged is called from canExecute sequence on scheduler.
        /// </summary>
        public RxCommand(IObservable<bool> canExecuteSource, IScheduler scheduler, bool initialValue = true)
            : base(canExecuteSource, scheduler, initialValue)
        {
        }

        /// <summary>
        /// Push null to subscribers.
        /// </summary>
        public void Execute()
        {
            Execute(null);
        }

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        public IDisposable Subscribe(Action onNext)
            => this.Subscribe(_ => onNext());
    }
}
