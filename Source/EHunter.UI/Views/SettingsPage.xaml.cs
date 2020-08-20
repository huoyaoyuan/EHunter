using EHunter.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.UI.Views
{
    public sealed partial class SettingsPage : Page
    {
        private readonly ICommonSetting _setting = Ioc.Default.GetRequiredService<ICommonSetting>();

        public SettingsPage() => InitializeComponent();
    }
}
