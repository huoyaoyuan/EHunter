using EHunter.Pixiv.ViewModels.User;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.User
{
    public sealed partial class UserTab : UserControl
    {
        public UserTab() => InitializeComponent();

        public static readonly DependencyProperty VMProperty
            = DependencyProperty.Register(nameof(VM), typeof(JumpToUserVM), typeof(UserTab),
                new PropertyMetadata(null, VMChanged));
        public JumpToUserVM VM
        {
            get => (JumpToUserVM)GetValue(VMProperty);
            set => SetValue(VMProperty, value);
        }

        private static void VMChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var userTab = (UserTab)sender;

            var connectedAnimationService = ConnectedAnimationService.GetForCurrentView();
            if (connectedAnimationService
                .GetAnimation("Username") is { } userNameAnimation)
            {
                userNameAnimation.TryStart(userTab.usernameText);
            }

            if (connectedAnimationService
                .GetAnimation("UserAvatar") is { } avatarAnimation)
            {
                avatarAnimation.TryStart(userTab.userAvatar);
            }
        }
    }
}
