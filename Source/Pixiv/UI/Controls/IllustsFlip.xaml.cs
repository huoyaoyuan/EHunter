using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable enable

namespace EHunter.Pixiv.Controls
{
    public sealed partial class IllustsFlip : UserControl
    {
        public IllustsFlip() => InitializeComponent();

        public static readonly DependencyProperty SourceListProperty
            = DependencyProperty.Register(nameof(SourceList), typeof(ListViewBase), typeof(IllustsFlip),
                new PropertyMetadata(null, (d, e) =>
                {
                    var c = (IllustsFlip)d;
                    if (e.OldValue is ListViewBase old)
                        old.ItemClick -= c.SourceList_ItemClick;
                    if (e.NewValue is ListViewBase @new)
                        @new.ItemClick += c.SourceList_ItemClick;
                }));
        public ListViewBase? SourceList
        {
            get => (ListViewBase?)GetValue(SourceListProperty);
            set => SetValue(SourceListProperty, value);
        }

        // Toggling self visibility will cause issue about initial selection.

        private void SourceList_ItemClick(object sender, ItemClickEventArgs e)
            => grid.Visibility = Visibility.Visible;

        private void CloseDetail()
            => grid.Visibility = Visibility.Collapsed;
    }
}
