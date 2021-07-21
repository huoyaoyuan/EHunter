using EHunter.Pixiv.ViewModels.Search;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.Search
{
    [DependencyProperty("VM", typeof(IllustSearchVM), IsNullable = true)]
    public sealed partial class IllustSearchTab : UserControl
    {
        public IllustSearchTab() => InitializeComponent();
    }
}
