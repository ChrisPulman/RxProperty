﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace Sample.ViewModels
{
    public class AsyncReactiveCommandViewModel : INotifyPropertyChanged
    {
        public AsyncReactiveCommandViewModel()
        {
            this.HeavyProcessCommand = new AsyncRxCommand();
            this.HeavyProcessCommand.Subscribe(HeavyWork);

            this.ShareSourceCommand1 = this.ShareSource.ToAsyncReactiveCommand();
            this.ShareSourceCommand1.Subscribe(async _ => await Task.Delay(500));
            this.ShareSourceCommand2 = this.ShareSource.ToAsyncReactiveCommand();
            this.ShareSourceCommand2.Subscribe(async _ => await Task.Delay(2000));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AsyncRxCommand HeavyProcessCommand { get; }

        public AsyncRxCommand ShareSourceCommand1 { get; }

        public AsyncRxCommand ShareSourceCommand2 { get; }

        private ReactiveProperty<bool> ShareSource { get; } = new ReactiveProperty<bool>(true);

        private static async Task HeavyWork()
        {
            await Task.Delay(3000);
        }
    }
}
