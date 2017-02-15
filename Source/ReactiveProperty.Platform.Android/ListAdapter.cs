using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace Reactive.Bindings
{
    /// <summary>
    /// 
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Convert IList to ListAdapter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">IList</param>
        /// <param name="createRowView">create view</param>
        /// <param name="setRowData">fill row data</param>
        /// <param name="getId">get id</param>
        /// <returns>ListAdapter</returns>
        public static ListAdapter<T> ToAdapter<T>(this IList<T> self, Func<int, T, View> createRowView, Action<int, T, View> setRowData, Func<int, T, long> getId = null) =>
            new ListAdapter<T>(self, createRowView, setRowData, getId);
    }

    /// <summary>
    /// Generic IList Adapter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListAdapter<T> : BaseAdapter<T>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="list">source list</param>
        /// <param name="createRowView">create view</param>
        /// <param name="setRowData">set row data</param>
        /// <param name="getId">get id</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/>"/> is <c>null</c>.</exception>
        public ListAdapter(IList<T> list, Func<int, T, View> createRowView, Action<int, T, View> setRowData, Func<int, T, long> getId = null)
        {
            this.List = list ?? throw new ArgumentNullException(nameof(list));
            this.CreateRowView = createRowView ?? throw new ArgumentNullException(nameof(createRowView));
            this.SetRowData = setRowData ?? throw new ArgumentNullException(nameof(setRowData));
            this.GetId = getId ?? ((index, _) => index);
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public override int Count => this.List.Count;

        private Func<int, T, View> CreateRowView { get; }

        private Func<int, T, long> GetId { get; }

        private IList<T> List { get; }

        private Action<int, T, View> SetRowData { get; }

        /// <summary>
        /// Gets the <see cref="T" /> with the specified error.
        /// </summary>
        /// <value>
        /// The <see cref="T" />.
        /// </value>
        /// <param name="ERROR">The error.</param>
        /// <returns></returns>
        public override T this[int position] => this.List[position];

        /// <summary>
        /// Gets the item identifier.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override long GetItemId(int position) => this.GetId(position, this[position]);

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="convertView">The convert view.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                convertView = this.CreateRowView(position, this[position]);
            }

            this.SetRowData(position, this[position], convertView);
            return convertView;
        }
    }
}