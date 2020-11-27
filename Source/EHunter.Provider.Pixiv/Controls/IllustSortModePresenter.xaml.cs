using Meowtrix.PixivApi;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Provider.Pixiv.Controls
{
    public sealed partial class IllustSortModePresenter : UserControl
    {
        public IllustSortModePresenter() => InitializeComponent();

        // TODO: https://github.com/microsoft/microsoft-ui-xaml/issues/3339
        private IllustSortMode _sortMode;
        public IllustSortMode SortMode
        {
            get => _sortMode;
            set
            {
                _sortMode = value;
                VisualStateManager.GoToState(this, value.ToString(), false);
            }
        }

        public int IntSortMode
        {
            get => (int)SortMode;
            set => SortMode = (IllustSortMode)value;
        }
    }
}
