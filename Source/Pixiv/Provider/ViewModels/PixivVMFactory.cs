using System.Collections.Generic;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.Media;
using EHunter.Pixiv.Media;
using EHunter.Pixiv.Services.Download;
using EHunter.Pixiv.ViewModels.Illusts;
using EHunter.Pixiv.ViewModels.User;
using EHunter.Services;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.Pixiv.ViewModels
{
    [Export, Shared]
    public class PixivVMFactory
    {
        private readonly IMemoryCache _memoryCache;
        private readonly DownloadManager _downloadManager;
        private readonly IViewModelService _viewModelService;
        private readonly ICustomResolver<PixivClient> _clientResolver;

        [ImportingConstructor]
        public PixivVMFactory(IMemoryCache memoryCache,
            DownloadManager downloadManager,
            IViewModelService viewModelService,
            ICustomResolver<PixivClient> clientResolver)
        {
            _memoryCache = memoryCache;
            _downloadManager = downloadManager;
            _viewModelService = viewModelService;
            _clientResolver = clientResolver;
        }

        public IImageSource GetImage(ImageInfo image) => new PixivImageSource(image, _memoryCache);
        public IImageSource GetAnimatedImage(Illust illust) => new AnimatedImageSource(illust, _memoryCache);

        public IllustVM CreateViewModel(Illust illust, int indexInCollection = -1)
            => new(illust, _downloadManager.GetOrAddDownloadable(illust), this, indexInCollection);

        [return: NotNullIfNotNull("source")]
        public IAsyncEnumerable<IllustVM>? CreateViewModels(IAsyncEnumerable<Illust>? source)
        {
            return source is null ? null : Core(source);
            // Select does ConfigureAwait(false)
            async IAsyncEnumerable<IllustVM> Core(IAsyncEnumerable<Illust> source)
            {
                int i = 1;
                await foreach (var illust in source.ConfigureAwait(true))
                    yield return CreateViewModel(illust, i++);
            }
        }

        [return: NotNullIfNotNull("source")]
        public IBindableCollection<IllustVM>? CreateAsyncCollection(IAsyncEnumerable<Illust>? source)
            => _viewModelService.CreateAsyncCollection(CreateViewModels(source));

        public UserWithPreviewVM CreateViewModel(UserInfoWithPreview userInfo) => new(userInfo, this);

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

        public JumpToUserVM JumpToUser() => new(_clientResolver.Resolve(), this);
        public JumpToUserVM JumpToUser(UserInfo userInfo) => new(_clientResolver.Resolve(), this, userInfo);
    }
}
