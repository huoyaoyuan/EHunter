using System;
using EHunter.UI.ViewModels;
using EHunter.UI.ViewModels.Messages;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.UI.Views
{
    public sealed partial class TabRootView : UserControl,
        IRecipient<NavigateMessage>,
        IRecipient<NavigateBackMessage>,
        IRecipient<NavigateForwardMessage>
    {
        public TabRootView()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                if (_viewModel != null)
                    Register(_viewModel);
            };

            Unloaded += (s, e) =>
            {
                if (_viewModel != null)
                    Unregister(_viewModel);
            };
        }

        private TabRootVM? _viewModel;
        public TabRootVM? ViewModel
        {
            get => _viewModel;
            set
            {
                if (_viewModel == value)
                    return;

                if (_viewModel != null)
                    Unregister(_viewModel);

                _viewModel = value;

                if (_viewModel != null && IsLoaded)
                    Register(_viewModel);
            }
        }

        private void Register(TabRootVM viewModel)
        {
            if (!Messenger.Default.IsRegistered<NavigateMessage, TabRootVM>(this, viewModel))
                Messenger.Default.RegisterAll(this, viewModel);
        }

        private void Unregister(TabRootVM viewModel)
        {
            if (Messenger.Default.IsRegistered<NavigateMessage, TabRootVM>(this, viewModel))
                Messenger.Default.UnregisterAll(this, viewModel);
        }

        void IRecipient<NavigateMessage>.Receive(NavigateMessage message)
        {
            _ = _frame.Navigate(message.Target switch
            {
                _ => throw new InvalidOperationException("Unsupported page type")
            }, message.Target);
        }

        void IRecipient<NavigateBackMessage>.Receive(NavigateBackMessage message)
        {
            if (_frame.CanGoBack)
                _frame.GoBack();
        }

        void IRecipient<NavigateForwardMessage>.Receive(NavigateForwardMessage message)
        {
            if (_frame.CanGoForward)
                _frame.GoForward();
        }
    }
}
