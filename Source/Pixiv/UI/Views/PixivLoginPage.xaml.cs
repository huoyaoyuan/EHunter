using EHunter.Pixiv.Messages;
using EHunter.Pixiv.ViewModels;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
#pragma warning disable CA1708
    public sealed partial class PixivLoginPage : Page
#pragma warning restore CA1708
    {
        private readonly PixivLoginPageVM _vm = Ioc.Default.GetRequiredService<PixivLoginPageVM>();

        public PixivLoginPage()
        {
            InitializeComponent();

            Loaded += (s, e) => WeakReferenceMessenger.Default.Register<PixivLoginPage, LoginFailedMessage>(
                this, static (self, m) =>
                {
                    self.loginFailedMessage.Text = m.Exception.Message;
                    FlyoutBase.ShowAttachedFlyout(self.loginPanel);
                });
            Unloaded += (s, e) => WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}
