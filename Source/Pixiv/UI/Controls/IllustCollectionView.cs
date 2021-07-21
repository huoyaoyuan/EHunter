using EHunter.Pixiv.ViewModels.Primitives;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Controls
{
    [DependencyProperty("ViewModel", typeof(IllustCollectionVM), IsNullable = true)]
    [DependencyProperty("IllustItemTemplate", typeof(DataTemplate), IsNullable = true)]
    [DependencyProperty("RefreshButtonVisibility", typeof(Visibility), DefaultValue = "Visibility.Visible")]
    [DependencyProperty("AgeSelectorVisibility", typeof(Visibility), DefaultValue = "Visibility.Visible")]
    public sealed partial class IllustCollectionView : ContentControl
    {
        public IllustCollectionView() => DefaultStyleKey = typeof(IllustCollectionView);
    }
}
