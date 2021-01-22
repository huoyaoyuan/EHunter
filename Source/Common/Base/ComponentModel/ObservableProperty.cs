using System;
using System.Collections.Generic;

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
                foreach (var o in _observers.ToArray())
                    o.OnNext(value);
            }
        }

        public IObservable<T> ValueObservable => this;

        private readonly List<IObserver<T>> _observers = new();
#pragma warning disable CA1033
        IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
#pragma warning restore CA1033
        {
            _observers.Add(observer);
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

            public void Dispose() => _owner._observers.Remove(_observer);
        }
    }
}
