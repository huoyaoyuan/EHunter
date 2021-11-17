using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Controls
{
    [DependencyProperty("SourceList", typeof(ListViewBase), IsNullable = true, InstanceChangedCallback = true)]
    public sealed partial class IllustsFlip : UserControl
    {
        public IllustsFlip() => InitializeComponent();

        partial void OnSourceListChanged(ListViewBase? oldValue, ListViewBase? newValue)
        {
            if (oldValue != null)
                oldValue.ItemClick -= SourceList_ItemClick;
            if (newValue != null)
                newValue.ItemClick += SourceList_ItemClick;
        }

        // Toggling self visibility will cause issue about initial selection.

        private void SourceList_ItemClick(object sender, ItemClickEventArgs e)
            => grid.Visibility = Visibility.Visible;

        private void CloseDetail()
            => grid.Visibility = Visibility.Collapsed;
    }
}
