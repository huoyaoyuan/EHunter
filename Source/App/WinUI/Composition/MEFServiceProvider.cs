using System;
using System.Composition.Hosting;

#nullable enable

namespace EHunter.UI.Composition
{
    internal class MEFServiceProvider : IServiceProvider
    {
        private readonly CompositionHost _host;

        public MEFServiceProvider(CompositionHost host) => _host = host;

        public object? GetService(Type serviceType)
            => _host.TryGetExport(serviceType, out object obj)
            ? obj : null;
    }
}
