
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.Pixiv.ViewModels.Primitives;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Search
{
    public partial class IllustSearchVM : IllustCollectionVM
    {
        private readonly IllustSearchManager _parent;

        [ObservableProperty]
        private Tag? _tag;

        [ObservableProperty]
        private string _searchWord = string.Empty;

        [ObservableProperty]
        private string _effectiveWord = string.Empty;

        [ObservableProperty]
        private IllustSearchTarget _searchTarget;

        [ObservableProperty]
        private IllustSortMode _sortMode;

        [ObservableProperty]
        private bool _minBookmarkEnabled;

        [ObservableProperty]
        private bool _maxBookmarkEnabled;

        [ObservableProperty]
        private int _minBookmark;

        [ObservableProperty]
        private int _maxBookmark;

        [ObservableProperty]
        private bool _startDateEnabled;

        [ObservableProperty]
        private bool _endDateEnabled;

        [ObservableProperty]
        private DateTimeOffset _startDate = DateTimeOffset.UtcNow.Date;

        [ObservableProperty]
        private DateTimeOffset _endDate = DateTimeOffset.UtcNow.Date;

        internal IllustSearchVM(IllustSearchManager parent)
            : base(parent.Factory) => _parent = parent;

        internal IllustSearchVM(IllustSearchManager parent, Tag tag)
            : base(parent.Factory)
        {
            _parent = parent;
            EffectiveWord = tag.Name;
            Tag = tag;
            Refresh();
        }

        protected override IAsyncEnumerable<Illust> LoadIllusts()
        {
            var options = new IllustFilterOptions
            {
                SortMode = SortMode,
                MinBookmarkCount = MinBookmarkEnabled ? MinBookmark : null,
                MaxBookmarkCount = MaxBookmarkEnabled ? MaxBookmark : null,
                StartDate = StartDateEnabled ? StartDate.DateTime : null,
                EndDate = EndDateEnabled ? EndDate.DateTime : null
            };

            if (Tag != null)
                EffectiveWord = SearchWord;

            return Tag != null
                ? Tag.GetIllustsAsync(options)
                : _parent.ClientResolver.Resolve().SearchIllustsAsync(SearchWord, SearchTarget, options);
        }
    }
}
