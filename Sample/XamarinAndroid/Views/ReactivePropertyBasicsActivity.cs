using System.Linq;
using System.Reactive.Linq;
using Android.App;
using Android.OS;
using Android.Widget;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sample.ViewModels;

namespace XamarinAndroid.Views
{
    [Activity(Label = "ReactivePropertyBasicsActivity")]
    public class ReactivePropertyBasicsActivity : Activity
    {
        private ReactivePropertyBasicsViewModel viewModel;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ReactivePropertyBasics);

            viewModel = new ReactivePropertyBasicsViewModel();

            FindViewById<EditText>(Resource.Id.EditTextInput)
                .SetBinding(
                    x => x.Text,
                    viewModel.InputText,
                    x => x.TextChangedAsObservable().ToUnit());

            FindViewById<TextView>(Resource.Id.TextViewOutput)
                .SetBinding(
                    x => x.Text,
                    viewModel.DisplayText);

            var buttonReplaceText = FindViewById<Button>(Resource.Id.ButtonReplaceText);
            buttonReplaceText
                .ClickAsObservable()
                .SetCommand(viewModel.ReplaceTextCommand);
            buttonReplaceText
                .SetBinding(
                    x => x.Enabled,
                    viewModel.ReplaceTextCommand
                        .CanExecuteChangedAsObservable()
                        .Select(_ => viewModel.ReplaceTextCommand.CanExecute())
                        .ToReactiveProperty());
        }
    }
}
