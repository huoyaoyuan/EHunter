using System.Composition;
using EHunter.DependencyInjection;
using EHunter.Pixiv.ViewModels.Illusts;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.User
{
    [Export, Shared]
    public class UserVMFactory
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;
        private readonly IllustVMFactory _illustVMFactory;

        [ImportingConstructor]
        public UserVMFactory(ICustomResolver<PixivClient> clientResolver,
            IllustVMFactory illustVMFactory)
        {
            _clientResolver = clientResolver;
            _illustVMFactory = illustVMFactory;
        }

        public JumpToUserVM Create() => new(_clientResolver.Resolve(), _illustVMFactory);
        public JumpToUserVM Create(UserInfo userInfo) => new(_clientResolver.Resolve(), _illustVMFactory, userInfo);
    }
}
