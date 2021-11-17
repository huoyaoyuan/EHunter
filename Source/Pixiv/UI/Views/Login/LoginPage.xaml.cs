using EHunter.Pixiv.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.Login
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [DependencyProperty("_vm", typeof(LoginPageVM))]
    public sealed partial class LoginPage : Page
    {
        public LoginPage() => InitializeComponent();

        private void DoPasswordLogin(object sender, RoutedEventArgs e)
        {
            _vm.LoginWithWebView(async url =>
            {
                var dialog = new BrowserLoginDialog
                {
                    XamlRoot = XamlRoot,
                    NavigateUrl = new Uri(url)
                };
                await dialog.ShowAsync();

                return dialog.ResultUrl ?? throw new TaskCanceledException();
            });
        }
    }
}
