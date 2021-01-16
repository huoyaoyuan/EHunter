using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;
using EHunter.ComponentModel;
using EHunter.Pixiv.ViewModels.Download;
using EHunter.Services;
using Meowtrix.PixivApi.Models;

#nullable enable

namespace EHunter.Pixiv.ViewModels
{
    public class IllustVM
    {
        internal IllustVM(Illust illust, IllustDownloadVM downloadable, int indexInCollection = -1)
        {
            Illust = illust;
            Downloadable = downloadable;
            IndexInCollection = indexInCollection;
        }

        public int? IndexInCollection { get; }

        public Illust Illust { get; }
        public IllustDownloadVM Downloadable { get; }

        public ImageInfo Preview => Illust.Pages[0].Medium;

        public string CreationTimeDisplayString => Illust.Created.ToLocalTime().ToString("f", CultureInfo.CurrentCulture);
    }

    public class IllustVMFactory
    {
        private readonly DownloadManager _downloadManager;
        private readonly IViewModelService _viewModelService;

        public IllustVMFactory(DownloadManager downloadManager, IViewModelService viewModelService)
        {
            _downloadManager = downloadManager;
            _viewModelService = viewModelService;
        }

        public IllustVM CreateViewModel(Illust illust, int indexInCollection = -1)
            => new(illust, _downloadManager.GetOrAddDownloadable(illust), indexInCollection);

        [return: NotNullIfNotNull("source")]
        public IAsyncEnumerable<IllustVM>? CreateViewModels(IAsyncEnumerable<Illust>? source)
        {
            return source is null ? null : Core(source);
            // Select does ConfigureAwait(false)
            async IAsyncEnumerable<IllustVM> Core(IAsyncEnumerable<Illust> source)
            {
                int i = 0;
                await foreach (var illust in source.ConfigureAwait(true))
                    yield return CreateViewModel(illust, i++);
            }
        }

        [return: NotNullIfNotNull("source")]
        public IBindableCollection<IllustVM>? CreateAsyncCollection(IAsyncEnumerable<Illust>? source)
            => _viewModelService.CreateAsyncCollection(CreateViewModels(source));
    }
}
