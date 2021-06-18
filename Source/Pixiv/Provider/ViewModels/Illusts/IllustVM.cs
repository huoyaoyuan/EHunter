using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EHunter.Pixiv.ViewModels.Download;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Illusts
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
}
