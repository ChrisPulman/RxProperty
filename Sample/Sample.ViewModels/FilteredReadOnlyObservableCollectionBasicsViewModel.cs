using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Helpers;

namespace Sample.ViewModels
{
    public class FilteredReadOnlyObservableCollectionBasicsViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Person> PeopleSource { get; } = new ObservableCollection<Person>();

        private bool UseRemovedFilter { get; set; }

        private Func<Person, bool> RemovedFilter { get; } = x => x.IsRemoved;

        private Func<Person, bool> NotRemovedFilter { get; } = x => !x.IsRemoved;

        public IFilteredReadOnlyObservableCollection<Person> People { get; }

        public RxCommand AddCommand { get; }

        public RxCommand RefreshCommand { get; }

        public FilteredReadOnlyObservableCollectionBasicsViewModel()
        {
            UseRemovedFilter = false;
            People = PeopleSource.ToFilteredReadOnlyObservableCollection(NotRemovedFilter);

            AddCommand = new RxCommand();
            AddCommand.Subscribe(_ => PeopleSource.Add(new Person()));

            RefreshCommand = new RxCommand();
            RefreshCommand.Subscribe(Refresh);
        }

        private void Refresh()
        {
            People.Refresh(UseRemovedFilter ? NotRemovedFilter : RemovedFilter);
            UseRemovedFilter = !UseRemovedFilter;
        }
    }

    public class Person : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) { return false; }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        private string name = $"tanaka-{DateTime.Now}";

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private bool isRemoved;

        public bool IsRemoved
        {
            get { return isRemoved; }
            set { SetProperty(ref isRemoved, value); }
        }
    }
}
