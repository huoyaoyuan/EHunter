﻿using EHunter.EHentai;
using EHunter.Pixiv;
using EHunter.UI.ViewModels;
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
            _serviceProvider = EHunterApp.Build(new PixivUIProvider(), new EHentaiUIProvider());

            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = new MainWindow()
            {
                ViewModel = _serviceProvider.GetRequiredService<MainWindowVM>()
            };
            _window.Activate();
        }

        private Window? _window;
        private readonly IServiceProvider _serviceProvider;
    }
}
