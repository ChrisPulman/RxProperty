using System;
using System.Windows;
using Reactive.Bindings;
using WPF.Views;

namespace WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            NavigateBasics = new RxCommand();
            NavigateBasics.Subscribe(_ => new ReactivePropertyBasics().Show());
            NavigateAsync = new RxCommand();
            NavigateAsync.Subscribe(_ => new Asynchronous().Show());
            NavigateValidation = new RxCommand();
            NavigateValidation.Subscribe(_ => new Validation().Show());
            NavigateEventToReactive = new RxCommand();
            NavigateEventToReactive.Subscribe(_ => new EventToReactiveProperty().Show());
            NavigateSynchronize = new RxCommand();
            NavigateSynchronize.Subscribe(_ => new SynchronizeObject().Show());
            NavigateEventToReactiveCommand = new RxCommand();
            NavigateEventToReactiveCommand.Subscribe(_ => new EventToReactiveCommandWindow().Show());
            NavigateAsyncReactiveCommand = new RxCommand();
            NavigateAsyncReactiveCommand.Subscribe(_ => new Views.AsyncReactiveCommand().Show());
            NavigateFilteredCollectionCommand = new RxCommand();
            NavigateFilteredCollectionCommand.Subscribe(_ => new Views.FilteredReadOnlyObservableCollectionBasics().Show());
        }

        public RxCommand NavigateAsync { get; private set; }

        public RxCommand NavigateAsyncReactiveCommand { get; }

        public RxCommand NavigateBasics { get; private set; }

        public RxCommand NavigateEventToReactive { get; private set; }

        public RxCommand NavigateEventToReactiveCommand { get; private set; }

        public RxCommand NavigateFilteredCollectionCommand { get; }

        public RxCommand NavigateSynchronize { get; private set; }

        public RxCommand NavigateValidation { get; private set; }
    }
}
