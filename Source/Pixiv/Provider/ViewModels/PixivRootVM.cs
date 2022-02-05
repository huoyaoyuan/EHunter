using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EHunter.Pixiv.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.Pixiv.ViewModels
{
    public sealed partial class PixivRootVM : ObservableObject, IRecipient<InitializationCompleteMessage>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessenger _messenger;

        public PixivRootVM(IServiceProvider serviceProvider, IMessenger messenger)
        {
            _serviceProvider = serviceProvider;
            _messenger = messenger;
            Login = _serviceProvider.GetRequiredService<LoginPageVM>();
            messenger.Register(this);
        }

        [ObservableProperty]
        private bool _isInitialized;

        public LoginPageVM Login { get; }

        [ObservableProperty]
        private PixivNavigationVM? _navigation;

        void IRecipient<InitializationCompleteMessage>.Receive(InitializationCompleteMessage message)
        {
            Navigation = new(_serviceProvider, _messenger);
            IsInitialized = true;
        }
    }
}
