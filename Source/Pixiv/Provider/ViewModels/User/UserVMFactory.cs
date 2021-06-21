using System.Collections.Generic;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.Services;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.User
{
    [Export, Shared]
    public class UserVMFactory
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;
        private readonly IViewModelService _viewModelService;
        private readonly PixivVMFactory _factory;

        [ImportingConstructor]
        public UserVMFactory(ICustomResolver<PixivClient> clientResolver,
            IViewModelService viewModelService,
            PixivVMFactory factory)
        {
            _clientResolver = clientResolver;
            _viewModelService = viewModelService;
            _factory = factory;
        }

        public JumpToUserVM Create() => new(_clientResolver.Resolve(), _factory);
        public JumpToUserVM Create(UserInfo userInfo) => new(_clientResolver.Resolve(), _factory, userInfo);

        public UserWithPreviewVM CreateViewModel(UserInfoWithPreview userInfo) => new(userInfo, _factory);

        [return: NotNullIfNotNull("source")]
        public IAsyncEnumerable<UserWithPreviewVM>? CreateViewModels(IAsyncEnumerable<UserInfoWithPreview>? source)
        {
            return source is null ? null : Core(source);
            // Select does ConfigureAwait(false)
            async IAsyncEnumerable<UserWithPreviewVM> Core(IAsyncEnumerable<UserInfoWithPreview> source)
            {
                await foreach (var user in source.ConfigureAwait(true))
                    yield return CreateViewModel(user);
            }
        }

        [return: NotNullIfNotNull("source")]
        public IBindableCollection<UserWithPreviewVM>? CreateAsyncCollection(IAsyncEnumerable<UserInfoWithPreview>? source)
            => _viewModelService.CreateAsyncCollection(CreateViewModels(source));
    }
}
