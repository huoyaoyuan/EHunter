using Microsoft.Extensions.DependencyInjection;

namespace EHunter.Providers
{
    public interface IEHunterProvider
    {
        string Title { get; }
        Uri IconUri { get; }
        object CreateUIRoot(IServiceProvider serviceProvider);
        void ConfigureServices(IServiceCollection serviceCollection);
    }
}
