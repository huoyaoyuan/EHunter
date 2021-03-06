﻿using CommunityToolkit.Mvvm.DependencyInjection;
using EHunter.Pixiv.ViewModels.Download;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.Download
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PixivDownloadPage : Page
    {
        private readonly AllDownloadsVM _vm = Ioc.Default.GetRequiredService<AllDownloadsVM>();

        public PixivDownloadPage() => InitializeComponent();
    }
}
