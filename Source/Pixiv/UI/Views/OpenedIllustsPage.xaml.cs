using EHunter.Pixiv.Messages;
using EHunter.Pixiv.ViewModels;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OpenedIllustsPage : Page
    {
        public OpenedIllustsPage() => InitializeComponent();

        private readonly OpenedIllustsVM _vm = Ioc.Default.GetRequiredService<OpenedIllustsVM>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e is
                {
                    NavigationMode: NavigationMode.New or NavigationMode.Forward,
                    Parameter: NavigateToIllustMessage m
                })
            {
                _vm.GoToIllust(m.Illust);
            }
        }

        private void JumpToUser_Clicked(object sender, RoutedEventArgs e)
        {
            var s = (FrameworkElement)sender;
            var avatar = (FrameworkElement)s.FindName("userAvatar");

            var connectedAnimationService = ConnectedAnimationService.GetForCurrentView();
            connectedAnimationService.PrepareToAnimate("Username", s);
            connectedAnimationService.PrepareToAnimate("UserAvatar", avatar);
        }
    }
}
