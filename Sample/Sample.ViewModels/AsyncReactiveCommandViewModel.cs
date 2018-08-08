using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.ViewModels
{
    public class AsyncReactiveCommandViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public AsyncRxCommand HeavyProcessCommand { get; }

        public AsyncRxCommand ShareSourceCommand1 { get; }

        public AsyncRxCommand ShareSourceCommand2 { get; }

        private ReactiveProperty<bool> ShareSource { get; } = new ReactiveProperty<bool>(true);

        public AsyncReactiveCommandViewModel()
        {
            this.HeavyProcessCommand = new AsyncRxCommand();
            this.HeavyProcessCommand.Subscribe(HeavyWork);

            this.ShareSourceCommand1 = this.ShareSource.ToAsyncReactiveCommand();
            this.ShareSourceCommand1.Subscribe(async _ => await Task.Delay(500));
            this.ShareSourceCommand2 = this.ShareSource.ToAsyncReactiveCommand();
            this.ShareSourceCommand2.Subscribe(async _ => await Task.Delay(2000));
        }

        private static async Task HeavyWork()
        {
            await Task.Delay(3000);
        }
    }
}
