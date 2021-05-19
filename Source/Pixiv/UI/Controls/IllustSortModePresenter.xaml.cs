using Meowtrix.PixivApi;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Controls
{
    public sealed partial class IllustSortModePresenter : UserControl
    {
        public IllustSortModePresenter() => InitializeComponent();

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
    }
}
