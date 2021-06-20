using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EHunter.Pixiv.Services.Images;
using EHunter.Pixiv.ViewModels.Download;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Illusts
{
    public class IllustVM
    {
        private readonly PixivImageService _imageService;

        internal IllustVM(Illust illust, IllustDownloadVM downloadable, PixivImageService imageService, int indexInCollection = -1)
        {
            Illust = illust;
            Downloadable = downloadable;
            _imageService = imageService;
            IndexInCollection = indexInCollection;
            Pages = illust.Pages.Select(x => new IllustPageVM(this, imageService, x)).ToArray();
        }

        public int? IndexInCollection { get; }

        public IReadOnlyList<IllustPageVM> Pages { get; }

        public Illust Illust { get; }
        public IllustDownloadVM Downloadable { get; }

        public ImageInfo Preview => Illust.Pages[0].Medium;

        public IllustPageVM PreviewPage => Pages[0];

        public IReadOnlyList<ImageVM> LargePages => Illust.IsAnimated
            ? new[] { new ImageVM(Illust, _imageService) }
            : Pages.Select(x => x.Large).ToArray();

        public IReadOnlyList<ImageVM> OriginalPages => Illust.IsAnimated
            ? new[] { new ImageVM(Illust, _imageService) }
            : Pages.Select(x => x.Original).ToArray();

        public string CreationTimeDisplayString => Illust.Created.ToLocalTime().ToString("f", CultureInfo.CurrentCulture);
    }
}
