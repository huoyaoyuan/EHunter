﻿using System.Composition;
using CommunityToolkit.Mvvm.Messaging;
using EHunter.Pixiv.Messages;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Export, Shared]
    public sealed partial class InitialPage : Page, IRecipient<InitializationCompleteMessage>
    {
        public InitialPage()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register(this);
        }

        void IRecipient<InitializationCompleteMessage>.Receive(InitializationCompleteMessage message)
            => presenter.Content = new NavigationPage();
    }
}
