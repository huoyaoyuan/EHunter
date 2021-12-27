using EHunter.Controls;
using EHunter.Pixiv.ViewModels.User;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.User
{
    public abstract class UserTabBase : UserControlFor<JumpToUserVM>
    {
    }

    public sealed partial class UserTab : UserTabBase
    {
        public UserTab() => InitializeComponent();
    }
}
