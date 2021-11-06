using EHunter.Controls;
using EHunter.Pixiv.ViewModels.User;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.User
{
    public abstract class UsersPageBase : PageFor<JumpToUserManager>
    {
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UsersPage : UsersPageBase
    {
        public UsersPage() => InitializeComponent();

        private void TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
            => ViewModel?.CloseTab(args.Item);
    }
}
