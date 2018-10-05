using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace XamarinAndroid.ViewModels
{
    public class ListAdapterActivityViewModel
    {
        private ObservableCollection<PersonViewModel> source = new ObservableCollection<PersonViewModel>();

        public ReadOnlyReactiveCollection<PersonViewModel> People { get; }

        public RxCommand AddPersonCommand { get; }

        public ListAdapterActivityViewModel(Activity context)
        {
            People = source.ToReadOnlyReactiveCollection();

            AddPersonCommand = new RxCommand();
            AddPersonCommand.Subscribe(_ => source.Add(new PersonViewModel(
                source.Count,
                "Person " + source.Count,
                30 + source.Count % 10)));
        }
    }

    public class PersonViewModel
    {
        public ReactiveProperty<long> Id { get; }

        public ReactiveProperty<string> Name { get; }

        public ReactiveProperty<string> Age { get; }

        private ReactiveTimer timer;

        public PersonViewModel(long id, string name, int age)
        {
            Id = new ReactiveProperty<long>(id);
            Name = new ReactiveProperty<string>(name);
            Age = new ReactiveProperty<string>(age.ToString());

            timer = new ReactiveTimer(TimeSpan.FromSeconds(10));
            timer
                .ObserveOnUIDispatcher()
                .Subscribe(_ => {
                    Age.Value = (int.Parse(Age.Value) + 1).ToString();
                });
            timer.Start();
        }
    }
}
