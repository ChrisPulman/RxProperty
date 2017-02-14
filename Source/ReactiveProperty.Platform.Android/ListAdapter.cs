using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace Reactive.Bindings
{
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
        public ListAdapter(IList<T> list, Func<int, T, View> createRowView, Action<int, T, View> setRowData, Func<int, T, long> getId = null)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }
            if (createRowView == null) { throw new ArgumentNullException(nameof(createRowView)); }
            if (setRowData == null) { throw new ArgumentNullException(nameof(setRowData)); }

            this.List = list;
            this.CreateRowView = createRowView;
            this.SetRowData = setRowData;
            this.GetId = getId ?? ((index, _) => index);
        }

        public override int Count => this.List.Count;

        private Func<int, T, View> CreateRowView { get; }

        private Func<int, T, long> GetId { get; }

        private IList<T> List { get; }

        private Action<int, T, View> SetRowData { get; }

        public override T this[int position] => this.List[position];

        public override long GetItemId(int position) => this.GetId(position, this[position]);

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