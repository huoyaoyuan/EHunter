using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.Pixiv.ViewModels
{
    [Export]
    public class PixivRootVM : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;

        [ImportingConstructor]
        public PixivRootVM(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Login = _serviceProvider.GetRequiredService<LoginPageVM>();
        }

        public LoginPageVM Login { get; }
        public PixivNavigationVM Navigation => new(_serviceProvider);
    }
}
