﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EHunter.Media;
using EHunter.Pixiv.ViewModels.Download;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Illusts
{
    public class IllustVM
    {
        private readonly PixivVMFactory _factory;

        internal IllustVM(Illust illust, IllustDownloadVM downloadable, PixivVMFactory factory, int indexInCollection = -1)
        {
            Illust = illust;
            Downloadable = downloadable;
            _factory = factory;
            IndexInCollection = indexInCollection;
            Pages = illust.Pages.Select(x => new IllustPageVM(this, factory, x)).ToArray();
        }

        public int? IndexInCollection { get; }

        public IReadOnlyList<IllustPageVM> Pages { get; }

        public Illust Illust { get; }
        public IllustDownloadVM Downloadable { get; }

        public IllustPageVM PreviewPage => Pages[0];

        public IReadOnlyList<IImageSource> LargePages => Illust.IsAnimated
            ? new[] { _factory.GetAnimatedImage(Illust) }
            : Pages.Select(x => x.Large).ToArray();

        public IReadOnlyList<IImageSource> OriginalPages => Illust.IsAnimated
            ? new[] { _factory.GetAnimatedImage(Illust) }
            : Pages.Select(x => x.Original).ToArray();

        public IImageSource UserAvatar => _factory.GetImage(Illust.User.Avatar);

        public string CreationTimeDisplayString => Illust.Created.ToLocalTime().ToString("f", CultureInfo.CurrentCulture);
    }
}
