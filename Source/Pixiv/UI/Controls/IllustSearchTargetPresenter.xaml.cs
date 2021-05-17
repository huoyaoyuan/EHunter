using Meowtrix.PixivApi;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Controls
{
    public sealed partial class IllustSearchTargetPresenter : UserControl
    {
        public IllustSearchTargetPresenter() => InitializeComponent();

        private IllustSearchTarget _searchTarget;
        public IllustSearchTarget SearchTarget
        {
            get => _searchTarget;
            set
            {
                _searchTarget = value;
                VisualStateManager.GoToState(this, value.ToString(), false);
            }
        }
    }
}
