using System.Composition;
using EHunter.UI.ViewModels;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable disable

namespace EHunter.UI.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Export]
    public sealed partial class MainWindow : Window
    {
        [Import]
        public MainWindowVM ViewModel { get; set; }

        [Import]
        public CompositionContext CompositionContext { get; set; }

        public MainWindow()
        {
            Title = "EHunter";
            InitializeComponent();
        }
    }
}
