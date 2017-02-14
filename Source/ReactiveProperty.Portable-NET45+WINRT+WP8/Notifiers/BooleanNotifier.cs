using System;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Reactive.Bindings.Notifiers
{
    /// <summary>
    /// Notify boolean flag.
    /// </summary>
    public class BooleanNotifier : IObservable<bool>, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        readonly Subject<bool> boolTrigger = new Subject<bool>();

        bool boolValue;
        /// <summary>Current flag value</summary>
        public bool Value
        {
            get { return this.boolValue; }
            set
            {
                this.boolValue = value;
                this.OnPropertyChanged();
                this.boolTrigger.OnNext(value);
            }
        }

        /// <summary>
        /// Setup initial flag.
        /// </summary>
        public BooleanNotifier(bool initialValue = false)
        {
            this.Value = initialValue;
        }

        /// <summary>
        /// Set and raise true if current value isn't true.
        /// </summary>
        public void TurnOn()
        {
            if (this.Value != true)
            {
                this.Value = true;
            }
        }

        /// <summary>
        /// Set and raise false if current value isn't false.
        /// </summary>
        public void TurnOff()
        {
            if (this.Value != false)
            {
                this.Value = false;
            }
        }

        /// <summary>
        /// Set and raise reverse value.
        /// </summary>
        public void SwitchValue() => this.Value = !this.Value;


        /// <summary>
        /// Subscribe observer.
        /// </summary>
        public IDisposable Subscribe(IObserver<bool> observer) => this.boolTrigger.Subscribe(observer);

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}