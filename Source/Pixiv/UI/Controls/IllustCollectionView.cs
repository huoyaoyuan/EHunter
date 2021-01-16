using EHunter.Pixiv.ViewModels.Primitives;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable enable

namespace EHunter.Pixiv.Controls
{
    public sealed class IllustCollectionView : ContentControl
    {
        public IllustCollectionView() => DefaultStyleKey = typeof(IllustCollectionView);

        public static readonly DependencyProperty ViewModelProperty
            = DependencyProperty.Register(nameof(ViewModel), typeof(IllustCollectionVM), typeof(IllustCollectionView),
                new PropertyMetadata(null));
        public IllustCollectionVM? ViewModel
        {
            get => (IllustCollectionVM?)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty IllustItemTemplateProperty
            = DependencyProperty.Register(nameof(IllustItemTemplate), typeof(DataTemplate), typeof(IllustCollectionView),
                new PropertyMetadata(null));
        public DataTemplate IllustItemTemplate
        {
            get => (DataTemplate)GetValue(IllustItemTemplateProperty);
            set => SetValue(IllustItemTemplateProperty, value);
        }
    }
}
