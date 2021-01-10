using EHunter.Pixiv.ViewModels.User;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.User
{
    public sealed partial class UserTabHeader : UserControl
    {
        public UserTabHeader() => InitializeComponent();

        public static readonly DependencyProperty VMProperty
            = DependencyProperty.Register(nameof(VM), typeof(JumpToUserVM), typeof(UserTab),
                new PropertyMetadata(null));
        public JumpToUserVM VM
        {
            get => (JumpToUserVM)GetValue(VMProperty);
            set => SetValue(VMProperty, value);
        }
    }
}
