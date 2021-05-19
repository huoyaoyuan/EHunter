using System.Composition;
using EHunter.ComponentModel;

namespace EHunter.EHentai.ViewModels.GalleryList
{
    [Export]
    public class GalleryListManager : TabsViewModel<GalleryListVM>
    {
        protected override GalleryListVM CreateNewTab() => new();
    }
}
