using System;
using System.Collections.Generic;
using EHunter.Pixiv.ViewModels.Primitives;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Search
{
    public class IllustSearchVM : IllustCollectionVM
    {
        private readonly IllustSearchManager _parent;

        internal IllustSearchVM(IllustSearchManager parent)
            : base(parent.IllustVMFactory) => _parent = parent;

        internal IllustSearchVM(IllustSearchManager parent, Tag tag)
            : base(parent.IllustVMFactory)
        {
            _parent = parent;
            EffectiveWord = tag.Name;
            Tag = tag;
            Refresh();
        }

        private Tag? _tag;
        public Tag? Tag
        {
            get => _tag;
            private set => SetProperty(ref _tag, value);
        }

        private string _searchWord = string.Empty;
        public string SearchWord
        {
            get => _searchWord;
            set => SetProperty(ref _searchWord, value);
        }

        private string _effectiveWord = string.Empty;
        public string EffectiveWord
        {
            get => _effectiveWord;
            private set => SetProperty(ref _effectiveWord, value);
        }

        public EnumValueHolder<IllustSearchTarget> SearchTarget { get; } = new();

        public EnumValueHolder<IllustSortMode> SortMode { get; } = new();

        private bool _minBookmarkEnabled;
        public bool MinBookmarkEnabled
        {
            get => _minBookmarkEnabled;
            set => SetProperty(ref _minBookmarkEnabled, value);
        }

        private bool _maxBookmarkEnabled;
        public bool MaxBookmarkEnabled
        {
            get => _maxBookmarkEnabled;
            set => SetProperty(ref _maxBookmarkEnabled, value);
        }

        private int _minBookmark;
        public int MinBookmark
        {
            get => _minBookmark;
            set => SetProperty(ref _minBookmark, value);
        }

        private int _maxBookmark;
        public int MaxBookmark
        {
            get => _maxBookmark;
            set => SetProperty(ref _maxBookmark, value);
        }

        private bool _startDateEnabled;
        public bool StartDateEnabled
        {
            get => _startDateEnabled;
            set => SetProperty(ref _startDateEnabled, value);
        }

        private bool _endDateEnabled;
        public bool EndDateEnabled
        {
            get => _endDateEnabled;
            set => SetProperty(ref _endDateEnabled, value);
        }

        private DateTimeOffset _startDate = DateTimeOffset.UtcNow.Date;
        public DateTimeOffset StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private DateTimeOffset _endDate = DateTimeOffset.UtcNow.Date;
        public DateTimeOffset EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        protected override IAsyncEnumerable<Illust> LoadIllusts()
        {
            var options = new IllustFilterOptions
            {
                SortMode = SortMode.Value,
                MinBookmarkCount = MinBookmarkEnabled ? MinBookmark : null,
                MaxBookmarkCount = MaxBookmarkEnabled ? MaxBookmark : null,
                StartDate = StartDateEnabled ? StartDate.DateTime : null,
                EndDate = EndDateEnabled ? EndDate.DateTime : null
            };

            if (Tag != null)
                EffectiveWord = SearchWord;

            return Tag != null
                ? Tag.GetIllustsAsync(options)
                : _parent.ClientResolver.Resolve().SearchIllustsAsync(SearchWord, SearchTarget.Value, options);
        }
    }
}
