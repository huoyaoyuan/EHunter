﻿using CommunityToolkit.Mvvm.DependencyInjection;
using EHunter.Pixiv.ViewModels.Bookmark;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.Bookmark
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyFollowingUsersPage : Page
    {
        private readonly MyFollowingVM _vm = Ioc.Default.GetRequiredService<MyFollowingVM>();
        public MyFollowingUsersPage() => InitializeComponent();
    }
}
