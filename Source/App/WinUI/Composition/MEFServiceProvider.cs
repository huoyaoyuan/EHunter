using System.Composition;

namespace EHunter.UI.Composition
{
    internal class MEFServiceProvider : IServiceProvider
    {
        private readonly CompositionContext _host;

        public MEFServiceProvider(CompositionContext host) => _host = host;

        public object? GetService(Type serviceType)
        {
            if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var originalType = serviceType.GetGenericArguments()[0];
                return _host.GetExports(originalType);
            }

            return _host.TryGetExport(serviceType, out object obj) ? obj : null;
        }
    }
}
