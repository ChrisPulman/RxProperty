using System;
using System.Collections.Generic;

namespace Reactive.Bindings.Extensions
{
    /// <summary>
    /// ///
    /// </summary>
    public static class IDisposableExtensions
    {
        /// <summary>
        /// Add disposable(self) to CompositeDisposable(or other ICollection)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="disposable">The disposable.</param>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        public static T AddTo<T>(this T disposable, ICollection<IDisposable> container)
            where T : IDisposable
        {
            container.Add(disposable);
            return disposable;
        }
    }
}