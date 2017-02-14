using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reactive.Bindings
{
    /// <summary>
    /// Represents AsyncReactiveCommand&lt;object&gt;
    /// </summary>
    public class AsyncReactiveCommand : AsyncReactiveCommand<object>
    {
        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncReactiveCommand()
            : base()
        {

        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncReactiveCommand(IObservable<bool> canExecuteSource)
            : base(canExecuteSource)
        {
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// The source is shared between other AsyncReactiveCommand.
        /// </summary>
        public AsyncReactiveCommand(IReactiveProperty<bool> sharedCanExecute)
            :base(sharedCanExecute)
        {
        }

        /// <summary>Push null to subscribers.</summary>
        public void Execute()
        {
            Execute(null);
        }
    }

    public class AsyncReactiveCommand<T> : ICommand, IDisposable
    {
        static readonly Task EmptyTask = Task.FromResult<object>(null);

        public event EventHandler CanExecuteChanged;

        readonly object gate = new object();
        readonly IReactiveProperty<bool> canExecute;
        readonly IDisposable sourceSubscription;
        bool isCanExecute;

        bool isDisposed = false;
        Notifiers.ImmutableList<Func<T, Task>> asyncActions = Notifiers.ImmutableList<Func<T, Task>>.Empty;

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncReactiveCommand()
        {
            this.canExecute = new ReactiveProperty<bool>(true);
            this.sourceSubscription = this.canExecute.Subscribe(x =>
            {
                this.isCanExecute = x;
                this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public AsyncReactiveCommand(IObservable<bool> canExecuteSource)
        {
            this.canExecute = new ReactiveProperty<bool>(true);
            this.sourceSubscription = this.canExecute.CombineLatest(canExecuteSource, (x, y) => x && y)
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    this.isCanExecute = x;
                    this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                });
        }

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// The source is shared between other AsyncReactiveCommand.
        /// </summary>
        public AsyncReactiveCommand(IReactiveProperty<bool> sharedCanExecute)
        {
            this.canExecute = sharedCanExecute;
            this.sourceSubscription = this.canExecute.Subscribe(x =>
            {
                this.isCanExecute = x;
                this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        /// <summary>Return current canExecute status.</summary>
        public bool CanExecute()
        {
            return this.isDisposed ? false : this.isCanExecute;
        }

        /// <summary>Return current canExecute status. parameter is ignored.</summary>
        bool ICommand.CanExecute(object parameter)
        {
            return this.isDisposed ? false : this.isCanExecute;
        }

        /// <summary>Push parameter to subscribers, when executing CanExecuting is changed to false.</summary>
        public async void Execute(T parameter)
        {
            if (this.isCanExecute)
            {
                this.canExecute.Value = false;
                var a = this.asyncActions.Data;
                if (a.Length == 1)
                {
                    try
                    {
                        var asyncState = a[0].Invoke(parameter) ?? EmptyTask;
                        await asyncState;
                    }
                    finally
                    {
                        this.canExecute.Value = true;
                    }
                }
                else
                {
                    var xs = new Task[a.Length];
                    try
                    {
                        for (int i = 0; i < a.Length; i++)
                        {
                            xs[i] = a[i].Invoke(parameter) ?? EmptyTask;
                        }

                        await Task.WhenAll(xs);
                    }
                    finally
                    {
                        this.canExecute.Value = true;
                    }
                }
            }
        }

        /// <summary>Push parameter to subscribers, when executing CanExecuting is changed to false.</summary>
        void ICommand.Execute(object parameter) => Execute((T)parameter);

        /// <summary>Subscribe execute.</summary>
        public IDisposable Subscribe(Func<T, Task> asyncAction)
        {
            lock (this.gate)
            {
                this.asyncActions = this.asyncActions.Add(asyncAction);
            }

            return new Subscription(this, asyncAction);
        }

        /// <summary>
        /// Stop all subscription and lock CanExecute is false.
        /// </summary>
        public void Dispose()
        {
            if (this.isDisposed) return;

            this.isDisposed = true;
            this.sourceSubscription.Dispose();
            if (this.isCanExecute)
            {
                this.isCanExecute = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        class Subscription : IDisposable
        {
            readonly AsyncReactiveCommand<T> parent;
            readonly Func<T, Task> asyncAction;

            public Subscription(AsyncReactiveCommand<T> parent, Func<T, Task> asyncAction)
            {
                this.parent = parent;
                this.asyncAction = asyncAction;
            }

            public void Dispose()
            {
                lock (this.parent.gate)
                {
                    this.parent.asyncActions = this.parent.asyncActions.Remove(this.asyncAction);
                }
            }
        }
    }

    public static class AsyncReactiveCommandExtensions
    {
        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public static AsyncReactiveCommand ToAsyncReactiveCommand(this IObservable<bool> canExecuteSource) =>
            new AsyncReactiveCommand(canExecuteSource);

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// </summary>
        public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this IObservable<bool> canExecuteSource) =>
            new AsyncReactiveCommand<T>(canExecuteSource);


        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// The source is shared between other AsyncReactiveCommand.
        /// </summary>
        public static AsyncReactiveCommand ToAsyncReactiveCommand(this IReactiveProperty<bool> sharedCanExecute) =>
            new AsyncReactiveCommand(sharedCanExecute);

        /// <summary>
        /// CanExecute is automatically changed when executing to false and finished to true.
        /// The source is shared between other AsyncReactiveCommand.
        /// </summary>
        public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(this IReactiveProperty<bool> sharedCanExecute) =>
            new AsyncReactiveCommand<T>(sharedCanExecute);
    }
}