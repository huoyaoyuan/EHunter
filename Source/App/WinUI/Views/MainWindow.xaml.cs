using EHunter.UI.ViewModels;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.UI.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private MainWindowVM? _viewModel;
        public MainWindowVM? ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                Bindings.Update();
            }
        }

        public MainWindow()
        {
            Title = "EHunter";
            InitializeComponent();
        }
    }
}
