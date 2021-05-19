using CommunityToolkit.Mvvm.DependencyInjection;
using EHunter.EHentai.ViewModels.GalleryList;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.EHentai.Views.GalleryList
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GalleryListPage : Page
    {
        private readonly GalleryListManager _vm = Ioc.Default.GetRequiredService<GalleryListManager>();

        public GalleryListPage() => InitializeComponent();

        private void TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
            => _vm.CloseTab(args.Item);
    }
}
