using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Collections;

namespace Sample.ViewModels
{
    public class ValidationViewModel
    {
        public ValidationViewModel()
        {
            // null is success(have no error), string is error message
            this.ValidationData = new ReactiveProperty<string>()
                .SetValidateNotifyError((string s) => !string.IsNullOrEmpty(s) && s.Cast<char>().All(Char.IsUpper) ? null : "not all uppercase");

            // async validation first argument is self observable sequence null is success(have no
            // error), IEnumerable is error messages
            this.ValidationNotify = new ReactiveProperty<string>("foo!", ReactivePropertyMode.RaiseLatestValueOnSubscribe)
                .SetValidateNotifyError(self => self
                    .Delay(TimeSpan.FromSeconds(3)) // asynchronous validation...
                    .Select(s => string.IsNullOrEmpty(s) ? null : "not empty string"));

            // both validation
            this.ValidationBoth = new ReactiveProperty<string>()
                .SetValidateNotifyError(s => !string.IsNullOrEmpty(s) && s.Cast<char>().All(Char.IsLower) ? null : "not all lowercase")
                .SetValidateNotifyError(self => self
                    .Delay(TimeSpan.FromSeconds(1)) // asynchronous validation...
                    .Select(s => string.IsNullOrEmpty(s) || s.Length <= 5 ? null : (IEnumerable)new[] { "length 5" }));

            // Validation result is pushed to ObserveErrors
            var errors = new[]
                {
                    this.ValidationData.ObserveErrorChanged,
                    this.ValidationBoth.ObserveErrorChanged,
                    this.ValidationNotify.ObserveErrorChanged
                }
                .CombineLatest();

            // Use OfType, choose error source
            this.ErrorInfo = errors
                .SelectMany(x => x)
                .Where(x => x != null)
                .Select(x => x.OfType<string>())
                .Select(x => x.FirstOrDefault())
                .ToReactiveProperty();
        }

        public ReactiveProperty<string> AlertMessage { get; }

        public ReactiveProperty<string> ErrorInfo { get; }

        public ReactiveProperty<string> ValidationBoth { get; }

        public ReactiveProperty<string> ValidationData { get; }

        public ReactiveProperty<string> ValidationNotify { get; }
    }
}