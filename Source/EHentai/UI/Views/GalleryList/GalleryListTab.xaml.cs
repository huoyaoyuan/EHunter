using EHunter.EHentai.ViewModels.GalleryList;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable enable

namespace EHunter.EHentai.Views.GalleryList
{
    public sealed partial class GalleryListTab : UserControl
    {
        public GalleryListTab() => InitializeComponent();

        public static readonly DependencyProperty VMProperty
            = DependencyProperty.Register(nameof(VM), typeof(GalleryListVM), typeof(GalleryListTab),
                new PropertyMetadata(null));
        public GalleryListVM? VM
        {
            get => (GalleryListVM?)GetValue(VMProperty);
            set => SetValue(VMProperty, value);
        }
    }
}
