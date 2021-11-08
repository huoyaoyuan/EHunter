using EHunter.Pixiv.ViewModels.User;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.User
{
    [DependencyProperty("VM", typeof(JumpToUserVM), IsNullable = true, InstanceChangedCallback = true)]
    public sealed partial class UserTab : UserControl
    {
        public UserTab() => InitializeComponent();

        partial void OnVMChanged(JumpToUserVM? oldValue, JumpToUserVM? newValue)
        {
            var connectedAnimationService = ConnectedAnimationService.GetForCurrentView();
            if (connectedAnimationService
                .GetAnimation("Username") is { } userNameAnimation)
            {
                userNameAnimation.TryStart(usernameText);
            }

            if (connectedAnimationService
                .GetAnimation("UserAvatar") is { } avatarAnimation)
            {
                avatarAnimation.TryStart(userAvatar);
            }
        }
    }
}
