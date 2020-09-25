using EHunter.Provider.Pixiv.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Provider.Pixiv.Views.User
{
    public sealed partial class UserTab : UserControl
    {
        public UserTab() => InitializeComponent();

        public static readonly DependencyProperty VMProperty
            = DependencyProperty.Register(nameof(VM), typeof(UserVM), typeof(UserTab),
                new PropertyMetadata(null, VMChanged));
        public UserVM VM
        {
            get => (UserVM)GetValue(VMProperty);
            set => SetValue(VMProperty, value);
        }

        private static void VMChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var userTab = (UserTab)sender;

            if (ConnectedAnimationService.GetForCurrentView()
                .GetAnimation("Username") is { } animation)
            {
                animation.TryStart(userTab.usernameText);
            }
        }
    }
}
