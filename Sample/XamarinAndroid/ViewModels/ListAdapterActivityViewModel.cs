using System;

using System;

using System.Collections.ObjectModel;
using Android.App;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace XamarinAndroid.ViewModels
{
    public class ListAdapterActivityViewModel
    {
        private ObservableCollection<PersonViewModel> source = new ObservableCollection<PersonViewModel>();

        public ListAdapterActivityViewModel(Activity context)
        {
            this.People = source.ToReadOnlyReactiveCollection();

            this.AddPersonCommand = new RxCommand();
            this.AddPersonCommand.Subscribe(_ => source.Add(new PersonViewModel(
                this.source.Count,
                "Person " + this.source.Count,
                30 + this.source.Count % 10)));
        }

        public RxCommand AddPersonCommand { get; }

        public ReadOnlyReactiveCollection<PersonViewModel> People { get; }
    }

    public class PersonViewModel
    {
        private ReactiveTimer timer;

        public PersonViewModel(long id, string name, int age)
        {
            this.Id = new ReactiveProperty<long>(id);
            this.Name = new ReactiveProperty<string>(name);
            this.Age = new ReactiveProperty<string>(age.ToString());

            this.timer = new ReactiveTimer(TimeSpan.FromSeconds(10));
            this.timer
                .ObserveOnUIDispatcher()
                .Subscribe(_ => {
                    this.Age.Value = (int.Parse(this.Age.Value) + 1).ToString();
                });
            this.timer.Start();
        }

        public ReactiveProperty<string> Age { get; }

        public ReactiveProperty<long> Id { get; }

        public ReactiveProperty<string> Name { get; }
    }
}
