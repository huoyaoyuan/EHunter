using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.EHentai.Api;

namespace EHunter.EHentai.ViewModels.GalleryList
{
    public class GalleryListManager : TabsViewModel<GalleryListVM>
    {
        private readonly ICustomResolver<EHentaiClient> _clientResolver;
        private readonly EHentaiVMFactory _factory;

        public GalleryListManager(ICustomResolver<EHentaiClient> clientResolver, EHentaiVMFactory factory)
        {
            _clientResolver = clientResolver;
            _factory = factory;
        }

        protected override GalleryListVM CreateNewTab() => new(_clientResolver.Resolve(), _factory);
    }
}
