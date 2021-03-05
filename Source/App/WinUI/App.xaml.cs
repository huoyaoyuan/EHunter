﻿using System.Composition.Convention;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using EHunter.Services;
using EHunter.UI.Composition;
using EHunter.UI.Services;
using EHunter.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;

#nullable enable

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
                });

            var convensionBuilder = new ConventionBuilder();
            convensionBuilder
                .ForType<ViewModelService>()
                .Export<IViewModelService>()
                .Shared();
            convensionBuilder
                .ForType<WinRTSettingsStore>()
                .Export<ISettingsStore>()
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

            Ioc.Default.ConfigureServices(new MEFServiceProvider(_host));

            InitializeComponent();
            Suspending += OnSuspending;
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

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            // Save application state and stop any background activity
        }

        private Window? _window;
        private readonly CompositionHost _host;
    }
}
