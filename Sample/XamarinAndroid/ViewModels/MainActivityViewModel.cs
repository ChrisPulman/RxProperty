using System;
using Android.App;
using Reactive.Bindings;

using System;

using XamarinAndroid.Views;

namespace XamarinAndroid.ViewModels
{
    public class MainActivityViewModel
    {
        public MainActivityViewModel(Activity context)
        {
            this.NavigateReactivePropertyBasicsCommand = new RxCommand();
            this.NavigateReactivePropertyBasicsCommand
                .Subscribe(_ => context.StartActivity(typeof(ReactivePropertyBasicsActivity)));

            this.NavigateListAdapterCommand = new RxCommand();
            this.NavigateListAdapterCommand
                .Subscribe(_ => context.StartActivity(typeof(ListAdapterActivity)));
        }

        public RxCommand NavigateListAdapterCommand { get; }

        public RxCommand NavigateReactivePropertyBasicsCommand { get; }
    }
}