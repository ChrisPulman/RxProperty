using System;

namespace System.ComponentModel
{
    /// <summary>
    /// System.ComponentModel.INotifyDataErrorInfo.ErrorsChanged イベントにデータを提供します。
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class DataErrorsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataErrorsChangedEventArgs"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public DataErrorsChangedEventArgs(string propertyName) => this.PropertyName = propertyName;

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public virtual string PropertyName { get; private set; }
    }
}