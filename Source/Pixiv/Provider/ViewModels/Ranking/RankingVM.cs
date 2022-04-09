using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.Pixiv.ViewModels.Primitives;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Ranking
{
    public partial class RankingVM : IllustCollectionVM
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;

        [ObservableProperty]
        private IllustRankingMode _selectedRankingMode;
        partial void OnSelectedRankingModeChanged(IllustRankingMode value) => Refresh();

        [ObservableProperty]
        private DateTimeOffset _date = DateTimeOffset.UtcNow.Date.AddDays(-1);
        partial void OnDateChanged(DateTimeOffset value) => Refresh();

        public RankingVM(ICustomResolver<PixivClient> clientResolver,
            PixivVMFactory factory)
            : base(factory)
            => _clientResolver = clientResolver;

        protected override IAsyncEnumerable<Illust>? LoadIllusts()
            => _clientResolver.Resolve().GetIllustRankingAsync(SelectedRankingMode, Date.Date);

        public void PrevDay() => Date -= TimeSpan.FromDays(1);

        public void NextDay() => Date += TimeSpan.FromDays(1);
    }
}
