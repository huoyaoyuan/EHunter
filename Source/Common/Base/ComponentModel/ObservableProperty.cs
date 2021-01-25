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

        private Action<T>? _actions;
        public IDisposable Subscribe(IObserver<T> observer)
        {
            _actions += observer.OnNext;
            observer.OnNext(Value);
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
