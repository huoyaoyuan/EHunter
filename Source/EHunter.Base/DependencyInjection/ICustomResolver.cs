namespace EHunter.DependencyInjection
{
    public interface ICustomResolver<out T> where T : class
    {
        T Resolve();
    }

    public static class ResolverExtensions
    {
        private class WrappedResolver<T> : ICustomResolver<T>
            where T : class
        {
            private readonly T _service;

            public WrappedResolver(T service) => _service = service;

            public T Resolve() => _service;
        }

        public static ICustomResolver<T> AsServiceResolver<T>(this T service)
            where T : class
            => new WrappedResolver<T>(service);
    }
}
