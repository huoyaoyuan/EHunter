using System.Composition;
using EHunter.UI.ViewModels;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.UI.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Export]
    public sealed partial class MainWindow : Window
    {
        private MainWindowVM? _viewModel;
        [Import]
        public MainWindowVM? ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                Bindings.Update();
            }
        }

        private CompositionContext? _compositionContext;
        [Import]
        public CompositionContext? CompositionContext
        {
            get => _compositionContext;
            set
            {
                _compositionContext = value;
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
