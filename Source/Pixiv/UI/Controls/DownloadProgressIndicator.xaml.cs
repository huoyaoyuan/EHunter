using EHunter.Pixiv.ViewModels.Download;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable enable

namespace EHunter.Pixiv.Controls
{
    public sealed partial class DownloadProgressIndicator : UserControl
    {
        public DownloadProgressIndicator() => InitializeComponent();

        private DownloadableVM2? _vm;
        public DownloadableVM2? VM
        {
            get => _vm;
            set
            {
                _vm = value;
                Bindings.Update();
            }
        }
    }
}
