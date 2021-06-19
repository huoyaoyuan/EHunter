using System.Collections.Generic;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using EHunter.ComponentModel;
using EHunter.Pixiv.Services.Images;
using EHunter.Pixiv.ViewModels.Download;
using EHunter.Services;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Illusts
{
    [Export, Shared]
    public class IllustVMFactory
    {
        private readonly DownloadManager _downloadManager;
        private readonly IViewModelService _viewModelService;
        private readonly PixivImageService _imageService;

        [ImportingConstructor]
        public IllustVMFactory(DownloadManager downloadManager,
            IViewModelService viewModelService,
            PixivImageService imageService)
        {
            _downloadManager = downloadManager;
            _viewModelService = viewModelService;
            _imageService = imageService;
        }

        public IllustVM CreateViewModel(Illust illust, int indexInCollection = -1)
            => new(illust, _downloadManager.GetOrAddDownloadable(illust), _imageService, indexInCollection);

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
    }
}
