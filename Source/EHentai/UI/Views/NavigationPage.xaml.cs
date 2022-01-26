using EHunter.EHentai.ViewModels;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.EHentai.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [DependencyProperty("ViewModel", typeof(EHentaiNavigationVM), IsNullable = true)]
    public sealed partial class NavigationPage : Page
    {
        public NavigationPage() => InitializeComponent();
    }
}
