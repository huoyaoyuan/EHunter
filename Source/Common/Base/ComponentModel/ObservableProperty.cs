using System;

namespace EHunter.ComponentModel
{
    public class ObservableProperty<T> : IObservable<T>
    {
        public ObservableProperty(T initialValue) => _value = initialValue;

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                _actions?.Invoke(value);
            }
        }

        public IObservable<T> ValueObservable => this;

        private Action<T>? _actions;
#pragma warning disable CA1033
        IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
#pragma warning restore CA1033
        {
            _actions += observer.OnNext;
            return new Subscriber(this, observer);
        }

        private class Subscriber : IDisposable
        {
            private readonly ObservableProperty<T> _owner;
            private readonly IObserver<T> _observer;

            public Subscriber(ObservableProperty<T> owner, IObserver<T> observer)
            {
                _owner = owner;
                _observer = observer;
            }

            public void Dispose() => _owner._actions -= _observer.OnNext;
        }
    }
}
