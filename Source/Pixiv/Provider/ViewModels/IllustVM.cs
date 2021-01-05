using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using EHunter.Pixiv.ViewModels.Download;
using Meowtrix.PixivApi.Models;

#nullable enable

namespace EHunter.Pixiv.ViewModels
{
    public class IllustVM
    {
        internal IllustVM(Illust illust, DownloadableIllustVM downloadable)
        {
            Illust = illust;
            Downloadable = downloadable;
        }

        public Illust Illust { get; }
        public DownloadableIllustVM Downloadable { get; }

        public ImageInfo Preview => Illust.Pages[0].Medium;

        public string CreationTimeDisplayString => Illust.Created.ToLocalTime().ToString("f", CultureInfo.CurrentCulture);
    }

    public class IllustVMFactory
    {
        private readonly DownloadManager _downloadManager;

        public IllustVMFactory(DownloadManager downloadManager) => _downloadManager = downloadManager;

        public IllustVM CreateViewModel(Illust illust) => new(illust, _downloadManager.GetDownloadableVM(illust));

        [return: NotNullIfNotNull("source")]
        public IAsyncEnumerable<IllustVM>? CreateViewModels(IAsyncEnumerable<Illust>? source)
            => source?.Select(CreateViewModel);
    }
}
