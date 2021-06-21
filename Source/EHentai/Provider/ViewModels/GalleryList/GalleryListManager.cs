using System.Composition;
using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.EHentai.Api;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.EHentai.ViewModels.GalleryList
{
    [Export]
    public class GalleryListManager : TabsViewModel<GalleryListVM>
    {
        private readonly ICustomResolver<EHentaiClient> _clientResolver;
        private readonly IMemoryCache _memoryCache;

        [ImportingConstructor]
        public GalleryListManager(ICustomResolver<EHentaiClient> clientResolver, IMemoryCache memoryCache)
        {
            _clientResolver = clientResolver;
            _memoryCache = memoryCache;
        }

        protected override GalleryListVM CreateNewTab() => new(_clientResolver.Resolve(), _memoryCache);
    }
}
