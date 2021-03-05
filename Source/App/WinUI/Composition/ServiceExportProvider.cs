using System;
using System.Collections.Generic;
using System.Composition.Hosting.Core;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

#nullable enable

namespace EHunter.UI.Composition
{
    internal class ServiceExportProvider : ExportDescriptorProvider
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly IServiceProvider _serviceProvider;

        public ServiceExportProvider(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
        {
            if (_serviceCollection.SingleOrDefault(x => x.ServiceType == contract.ContractType) is null)
            {
                return Array.Empty<ExportDescriptorPromise>();
            }

            return new[]
            {
                new ExportDescriptorPromise(contract, "ServiceProvider", false, NoDependencies,
                    _ => ExportDescriptor.Create(
                        (l, o)=> _serviceProvider.GetRequiredService(contract.ContractType),
                        NoMetadata))
            };
        }
    }
}
