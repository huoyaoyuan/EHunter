using System.Collections.Generic;
using EHunter.DependencyInjection;
using EHunter.Pixiv.ViewModels.Primitives;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Bookmark
{
    public class MyBookmarkVM : IllustCollectionVM
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;

        public MyBookmarkVM(ICustomResolver<PixivClient> clientResolver,
            IllustVMFactory illustVMFactory)
            : base(illustVMFactory) => _clientResolver = clientResolver;

        protected override IAsyncEnumerable<Illust>? LoadIllusts()
            => _clientResolver.Resolve().GetMyBookmarksAsync(Visibility.Public);
    }
}
