using EHunter.Provider.Pixiv.Messages;
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
                tabView.TabItems.Add(m.User);
            }
        }
    }
}
