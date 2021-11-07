using EHunter.Controls;
using EHunter.EHentai.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.EHentai.Views
{
    public abstract class EHentaiSettingsPageBase : PageFor<EHentaiSettingsVM>
    {
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EHentaiSettingsPage : EHentaiSettingsPageBase
    {
        public EHentaiSettingsPage() => InitializeComponent();
    }
}
