using System.Composition;
using EHunter.UI.ViewModels;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable enable

namespace EHunter.UI.Views
{
    [Export]
    public sealed partial class SettingsPage : Page
    {
        [Import]
        public CommonSettingVM? ViewModel { get; set; }

        public SettingsPage() => InitializeComponent();
    }
}
