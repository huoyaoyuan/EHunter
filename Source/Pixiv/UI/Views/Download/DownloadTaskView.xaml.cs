using EHunter.Pixiv.ViewModels.Download;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable enable

namespace EHunter.Pixiv.Views.Download
{
    public sealed partial class DownloadTaskView : UserControl
    {
        public DownloadTaskView() => InitializeComponent();

        private DownloadTaskVM? _vm;
        public DownloadTaskVM? VM
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
