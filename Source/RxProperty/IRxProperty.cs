using System.Reactive.Disposables;

namespace CP
{
    public interface IRxProperty<T> : IObservable<T?>, ICancelable
    {
        public T? Value { get; set; }
    }
}
