using System;
using System.Collections.Generic;
using EHunter.Pixiv.ViewModels.Primitives;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Search
{
    public partial class IllustSearchVM : IllustCollectionVM
    {
        private readonly IllustSearchManager _parent;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private Tag? _tag;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private string _searchWord = string.Empty;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private string _effectiveWord = string.Empty;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private IllustSearchTarget _searchTarget;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private IllustSortMode _sortMode;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private bool _minBookmarkEnabled;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private bool _maxBookmarkEnabled;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private int _minBookmark;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private int _maxBookmark;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private bool _startDateEnabled;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private bool _endDateEnabled;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private DateTimeOffset _startDate = DateTimeOffset.UtcNow.Date;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
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
