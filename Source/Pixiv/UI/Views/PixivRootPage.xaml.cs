using System.Composition;
using EHunter.Pixiv.ViewModels;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Export, Shared]
    [DependencyProperty("ViewModel", typeof(PixivRootVM))]
    public sealed partial class PixivRootPage : Page
    {
        public PixivRootPage() => InitializeComponent();
    }
}
