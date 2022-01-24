using EHunter.UI.ViewModels;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.UI.Views
{
    public sealed partial class SettingsPage : Page
    {
        private CommonSettingVM? _viewModel;
        public CommonSettingVM? ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                Bindings.Update();
            }
        }

        public SettingsPage() => InitializeComponent();
    }
}
