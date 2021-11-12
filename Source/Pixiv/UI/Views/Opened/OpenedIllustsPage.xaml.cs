using EHunter.Controls;
using EHunter.Pixiv.ViewModels.Opened;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.Opened
{
    public abstract class OpenedIllustsPageBase : PageFor<OpenedIllustsVM>
    {
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OpenedIllustsPage : OpenedIllustsPageBase
    {
        public OpenedIllustsPage() => InitializeComponent();
    }
}
