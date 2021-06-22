using System;
using System.Collections.Generic;
using EHunter.Pixiv.ViewModels.Primitives;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Search
{
    [ObservableProperty("Tag", typeof(Tag), IsNullable = true, IsSetterPublic = false)]
    [ObservableProperty("SearchWord", typeof(string), Initializer = "string.Empty")]
    [ObservableProperty("EffectiveWord", typeof(string), Initializer = "string.Empty", IsSetterPublic = false)]
    [ObservableProperty("SearchTarget", typeof(IllustSearchTarget))]
    [ObservableProperty("SortMode", typeof(IllustSortMode))]
    [ObservableProperty("MinBookmarkEnabled", typeof(bool))]
    [ObservableProperty("MaxBookmarkEnabled", typeof(bool))]
    [ObservableProperty("MinBookmark", typeof(int))]
    [ObservableProperty("MaxBookmark", typeof(int))]
    [ObservableProperty("StartDateEnabled", typeof(bool))]
    [ObservableProperty("EndDateEnabled", typeof(bool))]
    [ObservableProperty("StartDate", typeof(DateTimeOffset), Initializer = "DateTimeOffset.UtcNow.Date")]
    [ObservableProperty("EndDate", typeof(DateTimeOffset), Initializer = "DateTimeOffset.UtcNow.Date")]
    public partial class IllustSearchVM : IllustCollectionVM
    {
        private readonly IllustSearchManager _parent;

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
