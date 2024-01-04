// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Disposables;

namespace CP;

/// <summary>
/// Reactive Property.
/// </summary>
/// <typeparam name="T">The type of the property.</typeparam>
/// <seealso cref="IObservable&lt;T&gt;" />
/// <seealso cref="ICancelable" />
public interface IRxProperty<T> : IObservable<T?>, ICancelable
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public T? Value { get; set; }
}
