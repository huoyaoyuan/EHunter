using System.Globalization;
using System.Windows.Input;
using EHunter.Media;
using EHunter.Pixiv.Services.Download;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Illusts
{
    public class IllustVM
    {
        private readonly PixivVMFactory _factory;

        internal IllustVM(Illust illust, DownloadTask downloadable, PixivVMFactory factory, int indexInCollection = -1)
        {
            Illust = illust;
            Downloadable = downloadable;
            _factory = factory;
            IndexInCollection = indexInCollection;
            Pages = illust.Pages.Select(x => new IllustPageVM(this, factory, x)).ToArray();
            Tags = illust.Tags.Select(x => new IllustTagVM(factory, x)).ToArray();
        }

        public int? IndexInCollection { get; }

        public IReadOnlyList<IllustPageVM> Pages { get; }
        public IReadOnlyList<IllustTagVM> Tags { get; }

        public Illust Illust { get; }
        public DownloadTask Downloadable { get; }

        public IllustPageVM PreviewPage => Pages[0];

        public IReadOnlyList<IImageSource> LargePages => Illust.IsAnimated
            ? new[] { _factory.GetAnimatedImage(Illust) }
            : Pages.Select(x => x.Large).ToArray();

        public IReadOnlyList<IImageSource> OriginalPages => Illust.IsAnimated
            ? new[] { _factory.GetAnimatedImage(Illust) }
            : Pages.Select(x => x.Original).ToArray();

        public IImageSource UserAvatar => _factory.GetImage(Illust.User.Avatar);

        public ICommand NavigateToIllustCommand => _factory.NavigateToIllustCommand;
        public ICommand NavigateToUserCommand => _factory.NavigateToUserCommand;

        public string CreationTimeDisplayString => Illust.Created.ToLocalTime().ToString("f", CultureInfo.CurrentCulture);
    }
}
