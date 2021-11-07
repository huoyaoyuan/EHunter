using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.EHentai.ViewModels.GalleryList;
using Microsoft.Extensions.DependencyInjection;

namespace EHunter.EHentai.ViewModels
{
    [Export]
    public partial class EHentaiNavigationVM : ObservableObject
    {
        [ImportingConstructor]
        public EHentaiNavigationVM(IServiceProvider serviceProvider)
        {
            GalleryList = serviceProvider.GetRequiredService<GalleryListManager>();
            Settings = serviceProvider.GetRequiredService<EHentaiSettingsVM>();

            SelectedPage = GalleryList;
        }

        public GalleryListManager GalleryList { get; }

        public EHentaiSettingsVM Settings { get; }

        [ObservableProperty]
        private object? _selectedPage;
    }
}
