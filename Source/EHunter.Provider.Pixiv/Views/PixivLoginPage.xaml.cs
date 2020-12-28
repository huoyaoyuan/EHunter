using EHunter.Provider.Pixiv.ViewModels;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Provider.Pixiv.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
#pragma warning disable CA1708
    public sealed partial class PixivLoginPage : Page
#pragma warning restore CA1708
    {
        private readonly PixivLoginPageVM _vm = Ioc.Default.GetRequiredService<PixivLoginPageVM>();

        public PixivLoginPage() => InitializeComponent();
    }
}
