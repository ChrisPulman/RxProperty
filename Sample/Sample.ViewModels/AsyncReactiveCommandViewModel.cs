using System.ComponentModel;
using System.Threading.Tasks;
using Reactive.Bindings;

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
            HeavyProcessCommand = new AsyncRxCommand();
            HeavyProcessCommand.Subscribe(HeavyWork);

            ShareSourceCommand1 = ShareSource.ToAsyncReactiveCommand();
            ShareSourceCommand1.Subscribe(async _ => await Task.Delay(500));
            ShareSourceCommand2 = ShareSource.ToAsyncReactiveCommand();
            ShareSourceCommand2.Subscribe(async _ => await Task.Delay(2000));
        }

        private static Task HeavyWork()
        {
            return Task.Delay(3000);
        }
    }
}
