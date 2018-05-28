using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Microsoft.Win32;
using Reactive.Bindings.Interactivity;

using System;

namespace WPF.Views
{
    /// <summary>
    /// EventToReactiveCommandWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EventToReactiveCommandWindow : Window
    {
        public EventToReactiveCommandWindow()
        {
            InitializeComponent();
        }
    }

    // Converter
    public class OpenFileDialogConverter : ReactiveConverter<EventArgs, string>
    {
        protected override IObservable<string> OnConvert(IObservable<EventArgs> source)
        {
            return source
                .Select(_ => new OpenFileDialog())
                .Do(x => x.Filter = "*.*|*.*")
                .Where(x => x.ShowDialog() == true) // Show dialog
                .Select(x => x.FileName); // convert to string
        }
    }
}
