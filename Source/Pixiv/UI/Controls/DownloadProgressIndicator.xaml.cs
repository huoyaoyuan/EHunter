﻿using EHunter.Pixiv.Services.Download;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Controls
{
    public sealed partial class DownloadProgressIndicator : UserControl
    {
        public DownloadProgressIndicator() => InitializeComponent();

        private DownloadTask? _vm;
        public DownloadTask? VM
        {
            get => _vm;
            set
            {
                _vm = value;
                Bindings.Update();
            }
        }
    }
}
