using EHunter.Pixiv.ViewModels.Download;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable enable

namespace EHunter.Pixiv.Controls
{
    public sealed partial class DownloadButton : UserControl
    {
        private readonly DownloadManager _downloadManager = Ioc.Default.GetRequiredService<DownloadManager>();
        private DownloadableIllustVM? _vm;

        public DownloadButton() => InitializeComponent();

        private Illust? _illust;
        public Illust? Illust
        {
            get => _illust;
            set
            {
                _illust = value;
                _vm = value is null ? null
                    : _downloadManager.GetDownloadableVM(value);
                Bindings.Update();
            }
        }
    }
}
