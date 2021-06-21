using System;
using System.Collections.Generic;
using System.Composition;
using EHunter.DependencyInjection;
using EHunter.Pixiv.ViewModels.Primitives;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Ranking
{
    [Export]
    [ObservableProperty("SelectedRankingMode", typeof(IllustRankingMode), ChangedAction = "Refresh();")]
    [ObservableProperty("Date", typeof(DateTimeOffset), Initializer = "DateTimeOffset.UtcNow.Date.AddDays(-1)", ChangedAction = "Refresh();")]
    public partial class RankingVM : IllustCollectionVM
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;

        [ImportingConstructor]
        public RankingVM(ICustomResolver<PixivClient> clientResolver,
            PixivVMFactory factory)
            : base(factory)
        {
            _clientResolver = clientResolver;
            Refresh();
        }

        protected override IAsyncEnumerable<Illust>? LoadIllusts()
            => _clientResolver.Resolve().GetIllustRankingAsync(SelectedRankingMode, Date.Date);

        public void PrevDay() => Date -= TimeSpan.FromDays(1);

        public void NextDay() => Date += TimeSpan.FromDays(1);
    }
}
