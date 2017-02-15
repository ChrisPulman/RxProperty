using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Reactive.Bindings.Notifiers
{
    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public interface IAsyncMessageBroker : IAsyncMessagePublisher, IAsyncMessageSubscriber
    {
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public interface IAsyncMessagePublisher
    {
        /// <summary>
        /// Send Message to all receiver and await complete.
        /// </summary>
        Task PublishAsync<T>(T message);
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public interface IAsyncMessageSubscriber
    {
        /// <summary>
        /// Subscribe typed message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asyncAction">The asynchronous action.</param>
        /// <returns></returns>
        IDisposable Subscribe<T>(Func<T, Task> asyncAction);
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public interface IMessageBroker : IMessagePublisher, IMessageSubscriber
    {
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public interface IMessagePublisher
    {
        /// <summary>
        /// Send Message to all receiver.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        void Publish<T>(T message);
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public interface IMessageSubscriber
    {
        /// <summary>
        /// Subscribe typed message.
        /// </summary>
        IDisposable Subscribe<T>(Action<T> action);
    }

    /// <summary>
    /// Extensions of MessageBroker.
    /// </summary>
    public static class MessageBrokerExtensions
    {
        /// <summary>
        /// Convert IMessageSubscriber.Subscribe to Observable.
        /// </summary>
        public static IObservable<T> ToObservable<T>(this IMessageSubscriber messageSubscriber) =>
            Observable.Create<T>(observer =>
                {
                    var gate = new object();
                    IDisposable d = messageSubscriber.Subscribe<T>(x =>
                    {
                        // needs synchronize
                        lock (gate)
                        {
                            observer.OnNext(x);
                        }
                    });
                    return d;
                });
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public class AsyncMessageBroker : IAsyncMessageBroker, IDisposable
    {
        /// <summary>
        /// AsyncMessageBroker in Global scope.
        /// </summary>
        public static readonly IAsyncMessageBroker Default = new AsyncMessageBroker();

        private static readonly Task EmptyTask = Task.FromResult<object>(null);
        private readonly Dictionary<Type, object> notifiers = new Dictionary<Type, object>();
        private bool isDisposed = false;

        /// <summary>
        /// Stop Pub-Sub system.
        /// </summary>
        public void Dispose()
        {
            lock (this.notifiers)
            {
                if (!this.isDisposed)
                {
                    this.isDisposed = true;
                    this.notifiers.Clear();
                }
            }
        }

        /// <summary>
        /// Send Message to all receiver and await complete.
        /// </summary>
        public Task PublishAsync<T>(T message)
        {
            ImmutableList<Func<T, Task>> notifier;
            lock (this.notifiers)
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException("AsyncMessageBroker");
                }

                if (this.notifiers.TryGetValue(typeof(T), out var _notifier))
                {
                    notifier = (ImmutableList<Func<T, Task>>)_notifier;
                }
                else
                {
                    return EmptyTask;
                }
            }

            Func<T, Task>[] data = notifier.Data;
            Task[] awaiter = new Task[data.Length];
            for (var i = 0; i < data.Length; i++)
            {
                awaiter[i] = data[i].Invoke(message);
            }
            return Task.WhenAll(awaiter);
        }

        /// <summary>
        /// Subscribe typed message.
        /// </summary>
        public IDisposable Subscribe<T>(Func<T, Task> asyncAction)
        {
            lock (this.notifiers)
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException("AsyncMessageBroker");
                }

                if (!this.notifiers.TryGetValue(typeof(T), out var _notifier))
                {
                    ImmutableList<Func<T, Task>> notifier = ImmutableList<Func<T, Task>>.Empty;
                    notifier = notifier.Add(asyncAction);
                    this.notifiers.Add(typeof(T), notifier);
                }
                else
                {
                    var notifier = (ImmutableList<Func<T, Task>>)_notifier;
                    notifier = notifier.Add(asyncAction);
                    this.notifiers[typeof(T)] = notifier;
                }
            }

            return new Subscription<T>(this, asyncAction);
        }

        private class Subscription<T> : IDisposable
        {
            private readonly Func<T, Task> asyncAction;
            private readonly AsyncMessageBroker parent;

            public Subscription(AsyncMessageBroker parent, Func<T, Task> asyncAction)
            {
                this.parent = parent;
                this.asyncAction = asyncAction;
            }

            public void Dispose()
            {
                lock (this.parent.notifiers)
                {
                    if (this.parent.notifiers.TryGetValue(typeof(T), out var _notifier))
                    {
                        var notifier = (ImmutableList<Func<T, Task>>)_notifier;
                        notifier = notifier.Remove(this.asyncAction);

                        this.parent.notifiers[typeof(T)] = notifier;
                    }
                }
            }
        }
    }

    /// <summary>
    /// In-Memory PubSub filtered by Type.
    /// </summary>
    public class MessageBroker : IMessageBroker, IDisposable
    {
        /// <summary>
        /// MessageBroker in Global scope.
        /// </summary>
        public static readonly IMessageBroker Default = new MessageBroker();

        private readonly Dictionary<Type, object> notifiers = new Dictionary<Type, object>();
        private bool isDisposed = false;

        /// <summary>
        /// Stop Pub-Sub system.
        /// </summary>
        public void Dispose()
        {
            lock (this.notifiers)
            {
                if (!this.isDisposed)
                {
                    this.isDisposed = true;
                    this.notifiers.Clear();
                }
            }
        }

        /// <summary>
        /// Send Message to all receiver.
        /// </summary>
        public void Publish<T>(T message)
        {
            ImmutableList<Action<T>> notifier;
            lock (this.notifiers)
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException("AsyncMessageBroker");
                }

                if (this.notifiers.TryGetValue(typeof(T), out var _notifier))
                {
                    notifier = (ImmutableList<Action<T>>)_notifier;
                }
                else
                {
                    return;
                }
            }

            var data = notifier.Data;
            for (int i = 0; i < data.Length; i++)
            {
                data[i].Invoke(message);
            }
        }

        /// <summary>
        /// Subscribe typed message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public IDisposable Subscribe<T>(Action<T> action)
        {
            lock (this.notifiers)
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException("MessageBroker");
                }

                if (!this.notifiers.TryGetValue(typeof(T), out var _notifier))
                {
                    var notifier = ImmutableList<Action<T>>.Empty;
                    notifier = notifier.Add(action);
                    this.notifiers.Add(typeof(T), notifier);
                }
                else
                {
                    var notifier = (ImmutableList<Action<T>>)_notifier;
                    notifier = notifier.Add(action);
                    this.notifiers[typeof(T)] = notifier;
                }
            }

            return new Subscription<T>(this, action);
        }

        private class Subscription<T> : IDisposable
        {
            private readonly Action<T> action;
            private readonly MessageBroker parent;

            public Subscription(MessageBroker parent, Action<T> action)
            {
                this.parent = parent;
                this.action = action;
            }

            public void Dispose()
            {
                lock (this.parent.notifiers)
                {
                    if (this.parent.notifiers.TryGetValue(typeof(T), out var _notifier))
                    {
                        var notifier = (ImmutableList<Action<T>>)_notifier;
                        notifier = notifier.Remove(this.action);

                        this.parent.notifiers[typeof(T)] = notifier;
                    }
                }
            }
        }
    }

    // ImmutableList is from Rx internal
    internal class ImmutableList<T>
    {
        public static readonly ImmutableList<T> Empty = new ImmutableList<T>();

        private T[] data;

        public ImmutableList(T[] data) => this.data = data;

        private ImmutableList() => this.data = new T[0];

        public T[] Data
        {
            get { return this.data; }
        }

        public ImmutableList<T> Add(T value)
        {
            var newData = new T[this.data.Length + 1];
            Array.Copy(this.data, newData, this.data.Length);
            newData[this.data.Length] = value;
            return new ImmutableList<T>(newData);
        }

        public int IndexOf(T value)
        {
            for (var i = 0; i < this.data.Length; ++i)
            {
                if (object.Equals(this.data[i], value))
                {
                    return i;
                }
            }
            return -1;
        }

        public ImmutableList<T> Remove(T value)
        {
            var i = IndexOf(value);
            if (i < 0)
            {
                return this;
            }

            var length = this.data.Length;
            if (length == 1)
            {
                return Empty;
            }

            var newData = new T[length - 1];

            Array.Copy(this.data, 0, newData, 0, i);
            Array.Copy(this.data, i + 1, newData, i, length - i - 1);

            return new ImmutableList<T>(newData);
        }
    }
}