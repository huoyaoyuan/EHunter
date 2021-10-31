using System.Composition;
using EHunter.DependencyInjection;
using EHunter.Pixiv.ViewModels.Primitives;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Ranking
{
    [Export]
    public partial class RankingVM : IllustCollectionVM
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;

        // TODO: Custom source generation
        private IllustRankingMode _selectedRankingMode;
        public IllustRankingMode SelectedRankingMode
        {
            get => _selectedRankingMode;
            set
            {
                if (SetProperty(ref _selectedRankingMode, value))
                    Refresh();
            }
        }

        private DateTimeOffset _date = DateTimeOffset.UtcNow.Date.AddDays(-1);
        public DateTimeOffset Date
        {
            get => _date;
            set
            {
                if (SetProperty(ref _date, value))
                    Refresh();
            }
        }

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
