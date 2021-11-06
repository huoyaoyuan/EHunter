using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EHunter.Pixiv.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.Pixiv.ViewModels
{
    [Export]
    public sealed partial class PixivRootVM : ObservableObject, IRecipient<InitializationCompleteMessage>
    {
        private readonly IServiceProvider _serviceProvider;

        [ImportingConstructor]
        public PixivRootVM(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Login = _serviceProvider.GetRequiredService<LoginPageVM>();
            WeakReferenceMessenger.Default.Register(this);
        }

        [ObservableProperty]
        private bool _isInitialized;

        public LoginPageVM Login { get; }

        [ObservableProperty]
        private PixivNavigationVM? _navigation;

        void IRecipient<InitializationCompleteMessage>.Receive(InitializationCompleteMessage message)
        {
            Navigation = new(_serviceProvider);
            IsInitialized = true;
        }
    }
}
