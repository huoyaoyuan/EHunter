using System.Collections.Generic;
using System.Composition;
using EHunter.DependencyInjection;
using EHunter.Pixiv.ViewModels.Illusts;
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
            IllustVMFactory illustVMFactory)
            : base(illustVMFactory)
        {
            _clientResolver = clientResolver;
            SelectedAge = AgeRestriction.AllAge;
        }

        protected override IAsyncEnumerable<Illust>? LoadIllusts()
            => _clientResolver.Resolve().GetMyFollowingIllustsAsync();
    }
}
