using System;
using System.Collections.ObjectModel;
using EHunter.EHentai.Api;
using EHunter.EHentai.Api.Models;

namespace EHunter.EHentai.ViewModels.GalleryList
{
    public class GalleryListVM
    {
        private readonly EHentaiClient _client;

        public GalleryListVM(EHentaiClient client)
        {
            _client = client;
            LoadAsync();

            async void LoadAsync()
            {
                var page = await _client.GetPageAsync(new Uri("https://exhentai.org")).ConfigureAwait(true);
                foreach (var g in page)
                    Galleries.Add(g);
            }
        }

        public ObservableCollection<Gallery> Galleries { get; } = new();
    }
}
