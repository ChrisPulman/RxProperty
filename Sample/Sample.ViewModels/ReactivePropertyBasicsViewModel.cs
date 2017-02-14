using System;
using System.Reactive.Linq;
using System.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Concurrency; // using Namespace

namespace Sample.ViewModels
{
    // ReactiveProperty and ReactiveCommand simple example.
    public class ReactivePropertyBasicsViewModel
    {
        public ReactivePropertyBasicsViewModel()
        {
            // mode is Flags. (default is all) DistinctUntilChanged is no push value if next value is
            // same as current RaiseLatestValueOnSubscribe is push value when subscribed
            var allMode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe;

            // binding value from UI Control if no set initialValue then initialValue is default(T).
            // int:0, string:null...
            this.InputText = new ReactiveProperty<string>(initialValue: "", mode: allMode);

            // send value to UI Control
            this.DisplayText = this.InputText
                .Select(s => s.ToUpper())       // rx query1
                .ObserveOn(TaskPoolScheduler.Default)
                .Delay(TimeSpan.FromSeconds(1)) // rx query2
                .ObserveOnUIDispatcher()
                .ToReadOnlyReactiveProperty();          // convert to ReactiveProperty
        }

        public ReadOnlyReactiveProperty<string> DisplayText { get; }

        public ReactiveProperty<string> InputText { get; }
    }
}