using EHunter.Pixiv.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.IllustSearch
{
    public sealed partial class IllustSearchTab : UserControl
    {
        public IllustSearchTab() => InitializeComponent();

        public static readonly DependencyProperty VMProperty
            = DependencyProperty.Register(nameof(VM), typeof(IllustSearchVM), typeof(IllustSearchTab),
                new PropertyMetadata(null));
        public IllustSearchVM VM
        {
            get => (IllustSearchVM)GetValue(VMProperty);
            set => SetValue(VMProperty, value);
        }
    }
}
