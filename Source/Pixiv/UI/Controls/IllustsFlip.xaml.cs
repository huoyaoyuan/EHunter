using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Controls
{
    [DependencyProperty("SourceList", typeof(ListViewBase), IsNullable = true, ChangedMethod = nameof(OnSourceListChanged))]
    public sealed partial class IllustsFlip : UserControl
    {
        public IllustsFlip() => InitializeComponent();

        private static void OnSourceListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (IllustsFlip)d;
            if (e.OldValue is ListViewBase old)
                old.ItemClick -= c.SourceList_ItemClick;
            if (e.NewValue is ListViewBase @new)
                @new.ItemClick += c.SourceList_ItemClick;
        }

        // Toggling self visibility will cause issue about initial selection.

        private void SourceList_ItemClick(object sender, ItemClickEventArgs e)
            => grid.Visibility = Visibility.Visible;

        private void CloseDetail()
            => grid.Visibility = Visibility.Collapsed;
    }
}
