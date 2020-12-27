using System;
using System.Reactive.Linq;

namespace EHunter.ComponentModel
{
    public class ObservableProperty<T>
    {
        public ObservableProperty(T initialValue)
        {
            _value = initialValue;
            ValueObservable = Observable.FromEvent<T>(a => ValueChanged += a, a => ValueChanged -= a);
        }

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueChanged?.Invoke(value);
            }
        }

        private event Action<T>? ValueChanged;

        public IObservable<T> ValueObservable { get; }
    }
}
