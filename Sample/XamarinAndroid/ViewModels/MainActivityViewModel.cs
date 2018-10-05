using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Reactive.Bindings;
using XamarinAndroid.Views;

namespace XamarinAndroid.ViewModels
{
    public class MainActivityViewModel
    {
        public RxCommand NavigateReactivePropertyBasicsCommand { get; }

        public RxCommand NavigateListAdapterCommand { get; }

        public MainActivityViewModel(Activity context)
        {
            NavigateReactivePropertyBasicsCommand = new RxCommand();
            NavigateReactivePropertyBasicsCommand
                .Subscribe(_ => context.StartActivity(typeof(ReactivePropertyBasicsActivity)));

            NavigateListAdapterCommand = new RxCommand();
            NavigateListAdapterCommand
                .Subscribe(_ => context.StartActivity(typeof(ListAdapterActivity)));
        }
    }
}
