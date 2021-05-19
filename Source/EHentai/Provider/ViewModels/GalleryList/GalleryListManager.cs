using System.Composition;
using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.EHentai.Api;

namespace EHunter.EHentai.ViewModels.GalleryList
{
    [Export]
    public class GalleryListManager : TabsViewModel<GalleryListVM>
    {
        private readonly ICustomResolver<EHentaiClient> _clientResolver;

        [ImportingConstructor]
        public GalleryListManager(ICustomResolver<EHentaiClient> clientResolver) => _clientResolver = clientResolver;

        protected override GalleryListVM CreateNewTab() => new(_clientResolver.Resolve());
    }
}
