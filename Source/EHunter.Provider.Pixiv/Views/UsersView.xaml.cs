using System.Linq;
using EHunter.Provider.Pixiv.Messages;
using EHunter.Provider.Pixiv.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Provider.Pixiv.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UsersView : Page
    {
        public UsersView() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is NavigateToUserMessage m)
            {
                // TODO: SelectedItem doesn't work. SelectedIndex neither. Identify why in next WinUI preview.

                var vm = tabView.TabItems.Cast<UserSearchVM>().FirstOrDefault(x => x.UserInfo == m.User);
                if (vm is null)
                {
                    tabView.TabItems.Add(vm = new UserSearchVM { UserInfo = m.User });
                }

                tabView.SelectedItem = vm;

                //var tabItems = tabView.TabItems;
                //int index;
                //for (index = 0; index < tabItems.Count; index++)
                //    if (tabItems[index] is UserSearchVM vm && vm.UserInfo == m.User)
                //        break;

                //if (index == tabItems.Count)
                //    tabItems.Add(new UserSearchVM { UserInfo = m.User });

                //tabView.SelectedIndex = index;
            }
        }
    }
}
