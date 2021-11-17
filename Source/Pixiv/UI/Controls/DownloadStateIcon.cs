using EHunter.Pixiv.Services.Download;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Controls
{
    [DependencyProperty("State", typeof(IllustDownloadState), DefaultValue = "IllustDownloadState.NotLoaded", InstanceChangedCallback = true)]
    public sealed partial class DownloadStateIcon : Control
    {
        public DownloadStateIcon() => DefaultStyleKey = typeof(DownloadStateIcon);

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            VisualStateManager.GoToState(this, State.ToString(), true);
        }

        partial void OnStateChanged(IllustDownloadState oldValue, IllustDownloadState newValue)
            => VisualStateManager.GoToState(this, newValue.ToString(), true);
    }
}
