using System;
using EHunter.Provider.Pixiv.Messages;
using EHunter.Provider.Pixiv.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Provider.Pixiv.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PixivSettingsPage : Page
    {
        private readonly PixivSettings _setting = Ioc.Default.GetRequiredService<PixivSettings>();

        public PixivSettingsPage()
        {
            InitializeComponent();

            Loaded += (s, e) => WeakReferenceMessenger.Default.Register<LoginFailedMessage, PixivSettings>(this,
                _setting,
                async (o, m) =>
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Login Failed",
                        Content = m.Exception.Message,
                        XamlRoot = XamlRoot,
                        CloseButtonText = "OK"
                    };
                    await dialog.ShowAsync();
                });

            Unloaded += (s, e) => WeakReferenceMessenger.Default.UnregisterAll(this, _setting);
        }
    }
}
