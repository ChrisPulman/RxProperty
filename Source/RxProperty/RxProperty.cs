using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CP
{
    /// <summary>
    /// RxProperty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ReactiveUI.ReactiveObject" />
    /// <seealso cref="CP.IRxProperty&lt;T&gt;" />
    [DataContract]
    public class RxProperty<T> : ReactiveObject, IRxProperty<T>
    {
        private readonly IScheduler _scheduler;
        private readonly CompositeDisposable CleanUp = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="RxProperty{T}"/> class.
        /// </summary>
        public RxProperty()
        {
            _scheduler = RxApp.TaskpoolScheduler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RxProperty{T}"/> class.
        /// </summary>
        /// <param name="initialValue">The initial value.</param>
        public RxProperty(T? initialValue)
        {
            Value = initialValue;
            _scheduler = RxApp.TaskpoolScheduler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RxProperty{T}"/> class.
        /// </summary>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="scheduler">The scheduler.</param>
        public RxProperty(T? initialValue, IScheduler? scheduler)
        {
            Value = initialValue;
            _scheduler = scheduler ?? RxApp.TaskpoolScheduler;
        }

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        public bool IsDisposed => CleanUp.IsDisposed;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [DataMember]
        [Reactive]
        public T? Value { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Notifies the provider that an observer is to receive notifications.
        /// </summary>
        /// <param name="observer">The object that is to receive notifications.</param>
        /// <returns>
        /// A reference to an interface that allows observers to stop receiving notifications before
        /// the provider has finished sending them.
        /// </returns>
        public IDisposable Subscribe(IObserver<T?> observer) =>
            this.WhenAnyValue(vm => vm.Value)
            .ObserveOn(_scheduler)
            .Subscribe(observer)
            .DisposeWith(CleanUp);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (CleanUp?.IsDisposed == false && disposing) {
                CleanUp?.Dispose();
            }
        }
    }
}
