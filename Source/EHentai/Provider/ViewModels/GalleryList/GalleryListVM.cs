using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.EHentai.Api;
using EHunter.EHentai.Api.Models;

namespace EHunter.EHentai.ViewModels.GalleryList
{
    [ObservableProperty("CurrentPage", typeof(int), ChangedAction = "UpdatePage();", Initializer = "1")]
    [ObservableProperty("TotalPages", typeof(int))]
    [ObservableProperty("IsLoading", typeof(bool), IsSetterPublic = false)]
    [ObservableProperty("Galleries", typeof(IReadOnlyList<GalleryVM>), IsNullable = true, IsSetterPublic = false)]
    public partial class GalleryListVM : ObservableObject
    {
        private readonly EHentaiClient _client;

        public GalleryListVM(EHentaiClient client)
        {
            _client = client;
            UpdatePage();
        }

        private CancellationTokenSource? _cts;
        public async void UpdatePage()
        {
            _cts?.Cancel();
            Galleries = null;

            using var cts = _cts = new();
            IsLoading = true;
            try
            {
                var page = await _client.GetPageAsync(new ListRequest(), CurrentPage - 1, cts.Token)
                    .ConfigureAwait(true);
                cts.Token.ThrowIfCancellationRequested();

                TotalPages = page.PagesCount;
                Galleries = page.Galleries.Select(x => new GalleryVM(x)).ToArray();
            }
            catch
            {
            }
            finally
            {
                if (_cts == cts)
                {
                    _cts = null;
                    IsLoading = false;
                }
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
