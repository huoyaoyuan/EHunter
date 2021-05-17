using CommunityToolkit.Mvvm.Messaging;
using EHunter.Pixiv.Messages;
using EHunter.Pixiv.Views.Opened;
using EHunter.Pixiv.Views.Search;
using EHunter.Pixiv.Views.User;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable enable

namespace EHunter.Pixiv.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NavigationPage : Page,
        IRecipient<NavigateToUserMessage>,
        IRecipient<NavigateToIllustMessage>,
        IRecipient<NavigateToTagMessage>
    {
        public NavigationPage()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        void IRecipient<NavigateToUserMessage>.Receive(NavigateToUserMessage message)
        {
            _frame.Navigate(typeof(UsersPage), message);
            users.IsSelected = true;
        }

        void IRecipient<NavigateToIllustMessage>.Receive(NavigateToIllustMessage message)
        {
            _frame.Navigate(typeof(OpenedIllustsPage), message);
            opened.IsSelected = true;
        }

        void IRecipient<NavigateToTagMessage>.Receive(NavigateToTagMessage message)
        {
            _frame.Navigate(typeof(IllustSearchPage), message);
            searchIllust.IsSelected = true;
        }
    }
}
