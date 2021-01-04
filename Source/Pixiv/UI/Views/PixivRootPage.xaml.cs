using System;
using EHunter.Pixiv.Messages;
using EHunter.Pixiv.Views.Download;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable enable

namespace EHunter.Pixiv.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PixivRootPage : Page
    {
        public PixivRootPage()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<NavigateToUserMessage>(this,
                (o, m) =>
                {
                    _frame.Navigate(typeof(UsersPage), m);
                    users.IsSelected = true;
                });
            WeakReferenceMessenger.Default.Register<NavigateToIllustMessage>(this,
                (o, m) =>
                {
                    _frame.Navigate(typeof(OpenedIllustsPage), m);
                    opened.IsSelected = true;
                });
            WeakReferenceMessenger.Default.Register<NavigateToTagMessage>(this,
                (o, m) =>
                {
                    _frame.Navigate(typeof(IllustSearchPage), m);
                    searchIllust.IsSelected = true;
                });
        }

        private void NavigationView_SelectionChanged(
            NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            Type? toType = null;

            if (args.IsSettingsSelected)
                toType = typeof(PixivSettingsPage);
            else if (args.SelectedItemContainer == recent)
                toType = typeof(RecentPage);
            else if (args.SelectedItemContainer == users)
                toType = typeof(UsersPage);
            else if (args.SelectedItemContainer == opened)
                toType = typeof(OpenedIllustsPage);
            else if (args.SelectedItemContainer == searchIllust)
                toType = typeof(IllustSearchPage);
            else if (args.SelectedItemContainer == downloads)
                toType = typeof(PixivDownloadPage);

            if (toType is not null && toType != _frame.CurrentSourcePageType)
                _frame.Navigate(toType);
        }

        private void NavigationView_BackRequested(
            NavigationView sender,
            NavigationViewBackRequestedEventArgs args)
        {
            _frame.GoBack();

            var type = _frame.CurrentSourcePageType;
            if (type == typeof(PixivSettingsPage))
                sender.SelectedItem = sender.SettingsItem;
            else if (type == typeof(RecentPage))
                sender.SelectedItem = recent;
            else if (type == typeof(UsersPage))
                sender.SelectedItem = users;
            else if (type == typeof(OpenedIllustsPage))
                sender.SelectedItem = opened;
            else if (type == typeof(IllustSearchPage))
                sender.SelectedItem = searchIllust;
            else if (type == typeof(PixivDownloadPage))
                sender.SelectedItem = downloads;
        }
    }
}
