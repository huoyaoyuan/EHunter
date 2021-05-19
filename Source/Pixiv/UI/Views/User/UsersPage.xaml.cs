﻿using System.Linq;
using CommunityToolkit.Mvvm.DependencyInjection;
using EHunter.Pixiv.Messages;
using EHunter.Pixiv.ViewModels.User;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.User
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UsersPage : Page
    {
        private readonly UserVMFactory _factory = Ioc.Default.GetRequiredService<UserVMFactory>();
        private readonly JumpToUserManager _vm = Ioc.Default.GetRequiredService<JumpToUserManager>();

        public UsersPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e is
                {
                    NavigationMode: NavigationMode.New or NavigationMode.Forward,
                    Parameter: NavigateToUserMessage m
                })
            {
                var vm = _vm.Tabs.FirstOrDefault(x => x.UserInfo?.Id == m.User.Id);
                if (vm is null)
                {
                    _vm.Tabs.Add(vm = _factory.Create(m.User));
                }

                tabView.SelectedItem = vm;
            }
        }

        private void TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
            => _vm.CloseTab(args.Item);
    }
}
