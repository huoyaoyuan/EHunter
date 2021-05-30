using System.Composition.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.UI.Composition
{
    public static class MEFDependencyInjectionExtensions
    {
        public static ContainerConfiguration WithServiceCollection(
            this ContainerConfiguration container,
            IServiceCollection serviceCollection)
            => container.WithProvider(new ServiceExportProvider(serviceCollection));
    }
}
