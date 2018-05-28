using System;
using System.Linq;
using System.Reactive.Linq;
using Reactive.Bindings;

namespace Sample.ViewModels
{
    public class EventToReactiveCommandViewModel
    {
        public EventToReactiveCommandViewModel()
        {
            // command called, after converter
            this.SelectFileCommand = new RxCommand<string>();
            // create ReactiveProperty from ReactiveCommand
            this.Message = this.SelectFileCommand
                .Select(x => x + " selected.")
                .ToReactiveProperty();
        }

        public ReactiveProperty<string> Message { get; }

        public RxCommand<string> SelectFileCommand { get; }
    }
}
