using Microsoft.Extensions.DependencyInjection;

namespace EHunter.DependencyInjection
{
    public static class DependencyInjectionExtensions
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

        public static IServiceCollection AddConversion<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
            => serviceCollection.AddSingleton<TService>(sp => sp.GetRequiredService<TImplementation>());
    }
}
