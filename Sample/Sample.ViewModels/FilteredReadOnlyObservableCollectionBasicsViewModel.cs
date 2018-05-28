using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Reactive.Bindings;
using Reactive.Bindings.Helpers;

namespace Sample.ViewModels
{
    public class FilteredReadOnlyObservableCollectionBasicsViewModel : INotifyPropertyChanged
    {
        public FilteredReadOnlyObservableCollectionBasicsViewModel()
        {
            this.UseRemovedFilter = false;
            this.People = this.PeopleSource.ToFilteredReadOnlyObservableCollection(this.NotRemovedFilter);

            this.AddCommand = new RxCommand();
            this.AddCommand.Subscribe(_ => this.PeopleSource.Add(new Person()));

            this.RefreshCommand = new RxCommand();
            this.RefreshCommand.Subscribe(Refresh);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public RxCommand AddCommand { get; }

        public IFilteredReadOnlyObservableCollection<Person> People { get; }

        public RxCommand RefreshCommand { get; }

        private Func<Person, bool> NotRemovedFilter { get; } = x => !x.IsRemoved;

        private ObservableCollection<Person> PeopleSource { get; } = new ObservableCollection<Person>();

        private Func<Person, bool> RemovedFilter { get; } = x => x.IsRemoved;

        private bool UseRemovedFilter { get; set; }

        private void Refresh()
        {
            this.People.Refresh(this.UseRemovedFilter ? this.NotRemovedFilter : this.RemovedFilter);
            this.UseRemovedFilter = !this.UseRemovedFilter;
        }
    }

    public class Person : INotifyPropertyChanged
    {
        private bool isRemoved;

        private string name = $"tanaka-{DateTime.Now}";

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsRemoved
        {
            get { return this.isRemoved; }
            set { this.SetProperty(ref this.isRemoved, value); }
        }

        public string Name
        {
            get { return this.name; }
            set { this.SetProperty(ref this.name, value); }
        }

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) { return false; }

            field = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
