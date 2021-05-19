using Meowtrix.PixivApi;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Controls
{
    public sealed partial class IllustRankingModePresenter : UserControl
    {
        public IllustRankingModePresenter() => InitializeComponent();

        private IllustRankingMode _mode;
        public IllustRankingMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                Bindings.Update();
            }
        }
    }
}
