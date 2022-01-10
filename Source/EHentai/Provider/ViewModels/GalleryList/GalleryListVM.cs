using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.EHentai.Api;
using EHunter.EHentai.Api.Models;

namespace EHunter.EHentai.ViewModels.GalleryList
{
    public partial class GalleryListVM : ObservableObject
    {
        private readonly EHentaiClient _client;
        private readonly EHentaiVMFactory _factory;

        // TODO: Custom source generation
        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                    UpdatePage();
            }
        }

        [ObservableProperty]
        private int _totalPages;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private IReadOnlyList<GalleryVM>? _galleries;

        public GalleryListVM(EHentaiClient client, EHentaiVMFactory factory)
        {
            _client = client;
            _factory = factory;
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
                Galleries = page.Galleries.Select(x => new GalleryVM(x, _factory)).ToArray();
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
