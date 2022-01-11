using EHunter.Controls;
using EHunter.Pixiv.ViewModels.Search;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.Search
{
    public abstract class IllustSearchTabBase : UserControlFor<IllustSearchVM>
    {
    }

    public sealed partial class IllustSearchTab : IllustSearchTabBase
    {
        public IllustSearchTab() => InitializeComponent();
    }
}
