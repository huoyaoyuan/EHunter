using System;
using EHunter.Provider.Pixiv.Messages;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable enable

namespace EHunter.Provider.Pixiv.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PixivRootPage : Page
    {
        public PixivRootPage()
        {
            InitializeComponent();

            Messenger.Default.Register<NavigateToUserMessage>(this,
                m =>
                {
                    _frame.Navigate(typeof(UsersView), m);
                    users.IsSelected = true;
                });
        }

#pragma warning disable CA1801 // TODO: false positive - used in xaml event handler

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
                toType = typeof(UsersView);

            if (toType != null && toType != _frame.CurrentSourcePageType)
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
            else if (type == typeof(UsersView))
                sender.SelectedItem = users;
        }
    }
}
