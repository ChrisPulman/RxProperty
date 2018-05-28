using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

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
                return name;
            }

            set
            {
                name = value;
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
            TwoWay = inpc.ToReactivePropertyAsSynchronized(x => x.Name);

            // OneWay synchronize
            OneWay = inpc.ObserveProperty(x => x.Name).ToReactiveProperty();

            // OneWayToSource synchronize
            OneWayToSource = ReactiveProperty.FromObject(poco, x => x.Name);

            // synchronization check
            CheckCommand = new RxCommand();
            this.AlertMessage = CheckCommand.Select(_ =>
                "INPC Name:" + inpc.Name + Environment.NewLine
              + "POCO Name:" + poco.Name)
              .ToReactiveProperty(mode: ReactivePropertyMode.None);
        }

        public ReactiveProperty<string> AlertMessage { get; }

        public RxCommand CheckCommand { get; }

        public ReactiveProperty<string> OneWay { get; }

        public ReactiveProperty<string> OneWayToSource { get; }

        public ReactiveProperty<string> TwoWay { get; }
    }
}
