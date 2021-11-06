using System.Composition.Convention;
using System.Composition.Hosting;
using System.Reflection;
using EHunter.Services;
using EHunter.UI.Composition;
using EHunter.UI.Services;
using EHunter.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.UI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            var services = new ServiceCollection()
                .AddMemoryCache(o =>
                {
                    o.SizeLimit = 2 * (1L << 30);
                    o.CompactionPercentage = 0.9;
                })
                .AddSingleton<IViewModelService, ViewModelService>()
                .AddSingleton<ISettingsStore, WinRTSettingsStore>();

            var convensionBuilder = new ConventionBuilder();
            convensionBuilder
                .ForType<MEFServiceProvider>()
                .Export<IServiceProvider>()
                .Shared();

            _host = new ContainerConfiguration()
                .WithAssemblies(new[]
                    {
                        "EHunter.Base",
                        "EHunter.Common.UI",
                        "EHunter.Data",
                        "EHunter.Pixiv",
                        "EHunter.Pixiv.UI",
                        "EHunter.UI",
                    }
                    .Select(Assembly.Load))
                .WithDefaultConventions(convensionBuilder)
                .WithServiceCollection(services)
                .CreateContainer();

            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = _host.GetExport<MainWindow>();
            _window.Activate();
        }

        private Window? _window;
        private readonly CompositionHost _host;
    }
}
