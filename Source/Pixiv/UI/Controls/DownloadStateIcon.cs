using EHunter.Pixiv.Services.Download;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Controls
{
    [DependencyProperty("State", typeof(IllustDownloadState), ChangedMethod = "OnStateChanged")]
    public sealed partial class DownloadStateIcon : Control
    {
        public DownloadStateIcon() => DefaultStyleKey = typeof(DownloadStateIcon);

        private static void OnStateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState((DownloadStateIcon)sender,
                ((IllustDownloadState)e.NewValue).ToString(),
                true);
        }
    }
}
