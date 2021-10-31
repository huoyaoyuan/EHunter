using System.Composition;
using EHunter.DependencyInjection;
using EHunter.Pixiv.ViewModels.Primitives;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Recent
{
    [Export]
    public class RecentWatchedVM : IllustCollectionVM
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;

        [ImportingConstructor]
        public RecentWatchedVM(ICustomResolver<PixivClient> clientResolver,
            PixivVMFactory factory)
            : base(factory)
        {
            _clientResolver = clientResolver;
            SelectedAge = AgeRestriction.AllAge;
        }

        protected override IAsyncEnumerable<Illust>? LoadIllusts()
            => _clientResolver.Resolve().GetMyFollowingIllustsAsync();
    }
}
