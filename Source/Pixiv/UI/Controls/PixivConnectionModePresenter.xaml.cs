using EHunter.Pixiv.Settings;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Controls
{
    public sealed partial class PixivConnectionModePresenter : UserControl
    {
        public PixivConnectionModePresenter()
        {
            InitializeComponent();
            UpdateText();
        }

        private void UpdateText() => text.Text = Helpers.GetPixivConnectionMode(ConnectionMode);

        private PixivConnectionMode _connectionMode;
        public PixivConnectionMode ConnectionMode
        {
            get => _connectionMode;
            set
            {
                if (_connectionMode != value)
                {
                    _connectionMode = value;
                    UpdateText();
                }
            }
        }
    }
}
