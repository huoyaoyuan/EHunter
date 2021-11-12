using EHunter.Controls;
using EHunter.Pixiv.ViewModels.Download;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.Download
{
    public abstract class PixivDownloadPageBase : PageFor<AllDownloadsVM>
    {
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PixivDownloadPage : PixivDownloadPageBase
    {
        public PixivDownloadPage() => InitializeComponent();
    }
}
