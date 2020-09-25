using System.Collections.Generic;
using EHunter.Provider.Pixiv.ViewModels;
using Meowtrix.PixivApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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

        public static ImageInfo FirstPageMedium(IReadOnlyList<IllustPage> pages) => pages[0].Medium;

        private void GridView_ItemClick(object sender, ItemClickEventArgs e) => detailZone.Visibility = Visibility.Visible;

        private void CloseDetail() => detailZone.Visibility = Visibility.Collapsed;
    }
}
