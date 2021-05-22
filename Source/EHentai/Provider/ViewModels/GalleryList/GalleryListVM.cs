using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.EHentai.Api;
using EHunter.EHentai.Api.Models;

namespace EHunter.EHentai.ViewModels.GalleryList
{
    [ObservableProperty("CurrentPage", typeof(int), ChangedAction = "UpdatePage();", Initializer = "1")]
    [ObservableProperty("TotalPages", typeof(int))]
    [ObservableProperty("Galleries", typeof(IReadOnlyList<Gallery>), IsNullable = true, IsSetterPublic = false)]
    public partial class GalleryListVM : ObservableObject
    {
        private readonly EHentaiClient _client;

        public GalleryListVM(EHentaiClient client)
        {
            _client = client;
            UpdatePage();
        }

        public async void UpdatePage()
        {
            Galleries = null;
            try
            {
                var page = await _client.GetPageAsync(new ListRequest(), CurrentPage - 1).ConfigureAwait(true);
                TotalPages = page.PagesCount;
                Galleries = page.Galleries;
            }
            catch
            {
            }
        }

        public void PrevPage()
        {
            if (CurrentPage > 1)
                CurrentPage--;
        }

        public void NextPage()
        {
            if (CurrentPage < TotalPages)
                CurrentPage++;
        }
    }
}
