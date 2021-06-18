using System.Collections.Generic;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EHunter.ComponentModel;
using EHunter.Pixiv.ViewModels.Download;
using EHunter.Services;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels
{
    public class IllustVM
    {
        internal IllustVM(Illust illust, IllustDownloadVM downloadable, int indexInCollection = -1)
        {
            Illust = illust;
            Downloadable = downloadable;
            IndexInCollection = indexInCollection;
            Pages = illust.Pages.Select(x => new IllustPageVM(this, x)).ToArray();
        }

        public int? IndexInCollection { get; }

        public IReadOnlyList<IllustPageVM> Pages { get; }

        public Illust Illust { get; }
        public IllustDownloadVM Downloadable { get; }

        public ImageInfo Preview => Illust.Pages[0].Medium;

        public string CreationTimeDisplayString => Illust.Created.ToLocalTime().ToString("f", CultureInfo.CurrentCulture);
    }

    [Export, Shared]
    public class IllustVMFactory
    {
        private readonly DownloadManager _downloadManager;
        private readonly IViewModelService _viewModelService;

        [ImportingConstructor]
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
                int i = 1;
                await foreach (var illust in source.ConfigureAwait(true))
                    yield return CreateViewModel(illust, i++);
            }
        }

        [return: NotNullIfNotNull("source")]
        public IBindableCollection<IllustVM>? CreateAsyncCollection(IAsyncEnumerable<Illust>? source)
            => _viewModelService.CreateAsyncCollection(CreateViewModels(source));
    }
}
