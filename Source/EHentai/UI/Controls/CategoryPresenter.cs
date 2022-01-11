using EHunter.EHentai.Api;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.EHentai.Controls
{
    public sealed class CategoryPresenter : Control
    {
        public CategoryPresenter() => DefaultStyleKey = typeof(CategoryPresenter);

        public static readonly DependencyProperty CategoryProperty
            = DependencyProperty.Register(nameof(Category), typeof(GalleryCategory), typeof(CategoryPresenter),
                new PropertyMetadata(GalleryCategory.Misc, (d, e) =>
                    VisualStateManager.GoToState((CategoryPresenter)d, e.NewValue.ToString(), true)));
        public GalleryCategory Category
        {
            get => (GalleryCategory)GetValue(CategoryProperty);
            set => SetValue(CategoryProperty, value);
        }
    }
}
