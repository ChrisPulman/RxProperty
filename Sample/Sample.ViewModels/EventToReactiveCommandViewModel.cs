using System.Linq;
using System.Reactive.Linq;
using Reactive.Bindings;

namespace Sample.ViewModels
{
    public class EventToReactiveCommandViewModel
    {
        public RxCommand<string> SelectFileCommand { get; }

        public ReactiveProperty<string> Message { get; }

        public EventToReactiveCommandViewModel()
        {
            // command called, after converter
            SelectFileCommand = new RxCommand<string>();
            // create ReactiveProperty from ReactiveCommand
            Message = SelectFileCommand
                .Select(x => x + " selected.")
                .ToReactiveProperty();
        }
    }
}
