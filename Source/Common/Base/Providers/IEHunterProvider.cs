using System;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.Providers
{
    public interface IEHunterProvider
    {
        string Title { get; }
        Uri IconUri { get; }
        Type UIRootType { get; }
        void ConfigureServices(IServiceCollection serviceCollection);
    }
}
