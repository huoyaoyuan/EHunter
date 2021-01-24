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
    public sealed partial class LoginPage : Page, IRecipient<LoginFailedMessage>
#pragma warning restore CA1708
    {
        private readonly LoginPageVM _vm = Ioc.Default.GetRequiredService<LoginPageVM>();

        public LoginPage()
        {
            InitializeComponent();

            Loaded += (s, e) => WeakReferenceMessenger.Default.Register(this);
            Unloaded += (s, e) => WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        void IRecipient<LoginFailedMessage>.Receive(LoginFailedMessage message)
        {
            loginFailedMessage.Text = message.Exception.Message;
            FlyoutBase.ShowAttachedFlyout(loginPanel);
        }
    }
}
