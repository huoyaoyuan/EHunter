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
        internal IllustVM(Illust illust, IllustDownloadVM downloadable, PixivImageService imageService, int indexInCollection = -1)
        {
            Illust = illust;
            Downloadable = downloadable;
            IndexInCollection = indexInCollection;
            Pages = illust.Pages.Select(x => new IllustPageVM(this, imageService, x)).ToArray();
        }

        public int? IndexInCollection { get; }

        public IReadOnlyList<IllustPageVM> Pages { get; }

        public Illust Illust { get; }
        public IllustDownloadVM Downloadable { get; }

        public ImageInfo Preview => Illust.Pages[0].Medium;

        public IllustPageVM PreviewPage => Pages[0];

        public string CreationTimeDisplayString => Illust.Created.ToLocalTime().ToString("f", CultureInfo.CurrentCulture);
    }
}
