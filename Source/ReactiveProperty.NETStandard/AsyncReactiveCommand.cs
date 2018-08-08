using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reactive.Bindings
{
    /// <summary>
    /// AsyncReactiveCommand factory and extension methods.
    /// </summary>
    public static class AsyncReactiveCommandExtensions
    {
        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public static AsyncRxCommand ToAsyncReactiveCommand(this IObservable<bool> canExecuteSource) =>
            new AsyncRxCommand(canExecuteSource);

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public static AsyncRxCommand<T> ToAsyncReactiveCommand<T>(this IObservable<bool> canExecuteSource) =>
            new AsyncRxCommand<T>(canExecuteSource);

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true. The
        /// source is shared between other AsyncReactiveCommand.
        /// </summary>
        public static AsyncRxCommand ToAsyncReactiveCommand(this IReactiveProperty<bool> sharedCanExecute) =>
            new AsyncRxCommand(sharedCanExecute);

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true. The
        /// source is shared between other AsyncReactiveCommand.
        /// </summary>
        public static AsyncRxCommand<T> ToAsyncReactiveCommand<T>(this IReactiveProperty<bool> sharedCanExecute) =>
            new AsyncRxCommand<T>(sharedCanExecute);

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        /// <param name="self">AsyncReactiveCommand</param>
        /// <param name="asyncAction">Action</param>
        /// <param name="postProcess">Handling of the subscription.</param>
        /// <returns>Same of self argument</returns>
        public static AsyncRxCommand WithSubscribe(this AsyncRxCommand self, Func<Task> asyncAction, Action<IDisposable> postProcess = null)
        {
            var d = self.Subscribe(asyncAction);
            postProcess?.Invoke(d);
            return self;
        }

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        /// <typeparam name="T">AsyncReactiveCommand type argument.</typeparam>
        /// <param name="self">AsyncReactiveCommand</param>
        /// <param name="asyncAction">Action</param>
        /// <param name="postProcess">Handling of the subscription.</param>
        /// <returns>Same of self argument</returns>
        public static AsyncRxCommand<T> WithSubscribe<T>(this AsyncRxCommand<T> self, Func<T, Task> asyncAction, Action<IDisposable> postProcess = null)
        {
            var d = self.Subscribe(asyncAction);
            postProcess?.Invoke(d);
            return self;
        }

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        /// <param name="self">AsyncReactiveCommand</param>
        /// <param name="asyncAction">Action</param>
        /// <param name="disposable">The return value of self.Subscribe(asyncAction)</param>
        /// <returns>Same of self argument</returns>
        public static AsyncRxCommand WithSubscribe(this AsyncRxCommand self, Func<Task> asyncAction, out IDisposable disposable)
        {
            disposable = self.Subscribe(asyncAction);
            return self;
        }

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        /// <typeparam name="T">AsyncReactiveCommand type argument.</typeparam>
        /// <param name="self">AsyncReactiveCommand</param>
        /// <param name="asyncAction">Action</param>
        /// <param name="disposable">The return value of self.Subscribe(asyncAction)</param>
        /// <returns>Same of self argument</returns>
        public static AsyncRxCommand<T> WithSubscribe<T>(this AsyncRxCommand<T> self, Func<T, Task> asyncAction, out IDisposable disposable)
        {
            disposable = self.Subscribe(asyncAction);
            return self;
        }
    }

    /// <summary>
    /// Represents AsyncReactiveCommand&lt;object&gt;
    /// </summary>
    public class AsyncRxCommand : AsyncRxCommand<object>
    {
        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncRxCommand()
            : base()
        {
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncRxCommand(IObservable<bool> canExecuteSource)
            : base(canExecuteSource)
        {
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true. The
        /// source is shared between other AsyncReactiveCommand.
        /// </summary>
        public AsyncRxCommand(IReactiveProperty<bool> sharedCanExecute)
            : base(sharedCanExecute)
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
        public IDisposable Subscribe(Func<Task> asyncAction)
            => this.Subscribe(async _ => await asyncAction());
    }

    /// <summary>
    /// Async version ReactiveCommand
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncRxCommand<T> : ICommand, IDisposable
    {
        private readonly IReactiveProperty<bool> canExecute;

        private readonly object gate = new object();

        private readonly IDisposable sourceSubscription;

        private Notifiers.ImmutableList<Func<T, Task>> asyncActions = Notifiers.ImmutableList<Func<T, Task>>.Empty;

        private bool isCanExecute;

        private bool isDisposed = false;

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncRxCommand()
        {
            this.canExecute = new ReactiveProperty<bool>(true);
            this.sourceSubscription = this.canExecute.Subscribe(x => {
                this.isCanExecute = x;
                this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncRxCommand(IObservable<bool> canExecuteSource)
        {
            this.canExecute = new ReactiveProperty<bool>(true);
            this.sourceSubscription = canExecute.CombineLatest(canExecuteSource, (x, y) => x && y)
                .DistinctUntilChanged()
                .Subscribe(x => {
                    this.isCanExecute = x;
                    this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                });
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true. The
        /// source is shared between other AsyncReactiveCommand.
        /// </summary>
        public AsyncRxCommand(IReactiveProperty<bool> sharedCanExecute)
        {
            this.canExecute = sharedCanExecute;
            this.sourceSubscription = this.canExecute.Subscribe(x => {
                this.isCanExecute = x;
                this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Return current canExecute status.
        /// </summary>
        public bool CanExecute()
        {
            return isDisposed ? false : isCanExecute;
        }

        /// <summary>
        /// Return current canExecute status. parameter is ignored.
        /// </summary>
        bool ICommand.CanExecute(object parameter)
        {
            return isDisposed ? false : isCanExecute;
        }

        /// <summary>
        /// Stop all subscription and lock CanExecute is false.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;

            isDisposed = true;
            this.sourceSubscription.Dispose();
            if (isCanExecute) {
                isCanExecute = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Push parameter to subscribers, when executing CanExecuting is changed to false.
        /// </summary>
        public async void Execute(T parameter)
        {
            if (isCanExecute) {
                canExecute.Value = false;
                var a = asyncActions.Data;
                if (a.Length == 1) {
                    try {
                        var asyncState = a[0].Invoke(parameter) ?? Task.CompletedTask;
                        await asyncState;
                    } finally {
                        canExecute.Value = true;
                    }
                } else {
                    var xs = new Task[a.Length];
                    try {
                        for (int i = 0; i < a.Length; i++) {
                            xs[i] = a[i].Invoke(parameter) ?? Task.CompletedTask;
                        }

                        await Task.WhenAll(xs);
                    } finally {
                        canExecute.Value = true;
                    }
                }
            }
        }

        /// <summary>
        /// Push parameter to subscribers, when executing CanExecuting is changed to false.
        /// </summary>
        void ICommand.Execute(object parameter) => Execute((T)parameter);

        /// <summary>
        /// Subscribe execute.
        /// </summary>
        public IDisposable Subscribe(Func<T, Task> asyncAction)
        {
            lock (gate) {
                asyncActions = asyncActions.Add(asyncAction);
            }

            return new Subscription(this, asyncAction);
        }

        private class Subscription : IDisposable
        {
            private readonly Func<T, Task> asyncAction;
            private readonly AsyncRxCommand<T> parent;

            public Subscription(AsyncRxCommand<T> parent, Func<T, Task> asyncAction)
            {
                this.parent = parent;
                this.asyncAction = asyncAction;
            }

            public void Dispose()
            {
                lock (parent.gate) {
                    parent.asyncActions = parent.asyncActions.Remove(asyncAction);
                }
            }
        }
    }
}
