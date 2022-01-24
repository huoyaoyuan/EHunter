using System.Diagnostics.CodeAnalysis;
using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.EHentai.Api;
using EHunter.EHentai.Api.Models;
using EHunter.EHentai.Media;
using EHunter.EHentai.ViewModels.Galleries;
using EHunter.Media;
using EHunter.Services;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.EHentai.ViewModels
{
    public class EHentaiVMFactory
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICustomResolver<EHentaiClient> _clientResolver;
        private readonly IViewModelService _viewModelService;

        public EHentaiVMFactory(
            IMemoryCache memoryCache,
            ICustomResolver<EHentaiClient> clientResolver,
            IViewModelService viewModelService)
        {
            _memoryCache = memoryCache;
            _clientResolver = clientResolver;
            _viewModelService = viewModelService;
        }

        public GalleryVM GreateGallery(Gallery gallery) => new(gallery, this);

        public IImageSource GetThumbnail(Gallery gallery) => new ThumbnailSource(_clientResolver.Resolve(), gallery, _memoryCache);

        public IImageSource GetImagePreview(ImagePage image) => new ImagePreviewSource(_clientResolver.Resolve(), image, _memoryCache);

        public ImagePagePreviewVM CreatePreview(ImagePage image) => new(image, this);

        [return: NotNullIfNotNull("source")]
        public IAsyncEnumerable<ImagePagePreviewVM>? CreateViewModels(IAsyncEnumerable<ImagePage>? source)
        {
            return source is null ? null : Core(source);
            // Select does ConfigureAwait(false)
            async IAsyncEnumerable<ImagePagePreviewVM> Core(IAsyncEnumerable<ImagePage> source)
            {
                await foreach (var image in source.ConfigureAwait(true))
                    yield return CreatePreview(image);
            }
        }

        public IBindableCollection<ImagePagePreviewVM> CreateAsyncCollection(IAsyncEnumerable<ImagePage> images)
            => _viewModelService.CreateAsyncCollection(CreateViewModels(images));
    }
}
