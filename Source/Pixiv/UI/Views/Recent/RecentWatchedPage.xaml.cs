﻿using CommunityToolkit.Mvvm.DependencyInjection;
using EHunter.Pixiv.ViewModels.Recent;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.Recent
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecentWatchedPage : Page
    {
        private readonly RecentWatchedVM _vm = Ioc.Default.GetRequiredService<RecentWatchedVM>();

        public RecentWatchedPage() => InitializeComponent();

        private void NavigateToUser_Click(object sender, RoutedEventArgs args)
        {
            var button = (HyperlinkButton)sender;

            ConnectedAnimationService.GetForCurrentView()
                .PrepareToAnimate("Username", button);
        }
    }
}
