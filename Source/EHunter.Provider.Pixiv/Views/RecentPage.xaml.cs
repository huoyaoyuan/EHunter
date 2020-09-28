using EHunter.Provider.Pixiv.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Provider.Pixiv.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecentPage : Page
    {
        private readonly PixivRecentVM _vm = Ioc.Default.GetRequiredService<PixivRecentVM>();

        public RecentPage() => InitializeComponent();

        private void GridView_ItemClick(object sender, ItemClickEventArgs e) => detailZone.Visibility = Visibility.Visible;

        private void CloseDetail() => detailZone.Visibility = Visibility.Collapsed;

        private void NavigateToUser_Click(object sender, RoutedEventArgs args)
        {
            var button = (HyperlinkButton)sender;

            ConnectedAnimationService.GetForCurrentView()
                .PrepareToAnimate("Username", button);
        }
    }
}
