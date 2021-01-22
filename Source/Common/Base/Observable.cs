using System;

namespace EHunter
{
    public static class Observable
    {
        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> observerAction)
            => observable.Subscribe(new ActionObserver<T>(observerAction));

        private class ActionObserver<T> : IObserver<T>
        {
            private readonly Action<T> _action;

            public ActionObserver(Action<T> action) => _action = action;

            public void OnCompleted() { }
            public void OnError(Exception error) { }
            public void OnNext(T value) => _action(value);
        }
    }
}
