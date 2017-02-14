using System;
using System.Reactive.Linq;
using System.Windows;
using Reactive.Bindings;
using System.ComponentModel;
using Reactive.Bindings.Extensions; // using namespace

namespace Sample.ViewModels
{
    public class ObservableObject : INotifyPropertyChanged
    {
        private string name;

        public event PropertyChangedEventHandler PropertyChanged = (_, __) => { };

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }
    }

    public class PlainObject
    {
        public string Name { get; set; }
    }

    // Synchroinize exsiting models.
    public class SynchronizeObjectViewModel
    {
        public SynchronizeObjectViewModel()
        {
            var inpc = new ObservableObject { Name = "Bill" };
            var poco = new PlainObject { Name = "Steve" };

            // TwoWay synchronize
            this.TwoWay = inpc.ToReactivePropertyAsSynchronized(x => x.Name);

            // OneWay synchronize
            this.OneWay = inpc.ObserveProperty(x => x.Name).ToReactiveProperty();

            // OneWayToSource synchronize
            this.OneWayToSource = ReactiveProperty.FromObject(poco, x => x.Name);
        }

        public ReactiveProperty<string> AlertMessage { get; }

        public ReactiveProperty<string> OneWay { get; }

        public ReactiveProperty<string> OneWayToSource { get; }

        public ReactiveProperty<string> TwoWay { get; }
    }
}