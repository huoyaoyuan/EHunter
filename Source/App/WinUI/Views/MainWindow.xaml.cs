using EHunter.Providers;
using EHunter.UI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable disable

namespace EHunter.UI.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindowVM ViewModel { get; set; }

        public MainWindow()
        {
            Title = "EHunter";
            InitializeComponent();
        }

        private int _previousSelectedIndex = -1;

        private void NavigationView_SelectionChanged(
            NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            var targetType = args.IsSettingsSelected
                ? typeof(SettingsPage)
                : ((IEHunterProvider)args.SelectedItem).UIRootType;

            int newIndex = args.IsSettingsSelected
                ? int.MaxValue
                : ViewModel.Providers.IndexOf((IEHunterProvider)args.SelectedItem);

            var direction = newIndex > _previousSelectedIndex
                ? SlideNavigationTransitionEffect.FromRight
                : SlideNavigationTransitionEffect.FromLeft;

            _ = _frame.Navigate(targetType,
                null,
                new SlideNavigationTransitionInfo { Effect = direction });

            _previousSelectedIndex = newIndex;
        }
    }
}
