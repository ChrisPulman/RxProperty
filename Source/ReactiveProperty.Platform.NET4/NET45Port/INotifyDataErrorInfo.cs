using System;
using System.Collections;

namespace System.ComponentModel
{
    /// <summary>
    /// INotifyDataErrorInfo
    /// </summary>
    public interface INotifyDataErrorInfo
    {
        /// <summary>
        /// Occurs when [errors changed].
        /// </summary>
        event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value><c>true</c> if this instance has errors; otherwise, <c>false</c>.</value>
        bool HasErrors { get; }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        IEnumerable GetErrors(string propertyName);
    }
}