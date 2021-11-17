using EHunter.Controls;
using EHunter.Pixiv.ViewModels.Search;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.Search
{
    public abstract class IllustSearchPageBase : PageFor<IllustSearchManager>
    {
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IllustSearchPage : IllustSearchPageBase
    {
        public IllustSearchPage() => InitializeComponent();

        private void TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
            => ViewModel?.CloseTab(args.Item);
    }
}
