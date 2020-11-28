using System;
using System.Collections.ObjectModel;
using EHunter.ComponentModel;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace EHunter.Provider.Pixiv.ViewModels
{
    public class IllustSearchVM : ObservableObject
    {
        private readonly IllustSearchPageVM _parent;

        internal IllustSearchVM(IllustSearchPageVM parent) => _parent = parent;

        internal IllustSearchVM(IllustSearchPageVM parent, Tag tag)
        {
            _parent = parent;
            EffectiveWord = tag.Name;
            Tag = tag;
            Illusts = new(tag.GetIllustsAsync());
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

        private AgeRestriction _age = AgeRestriction.All;
        public AgeRestriction Age
        {
            get => _age;
            set => SetProperty(ref _age, value);
        }

        private IllustSearchTarget _searchTarget = IllustSearchTarget.PartialTag;
        public IllustSearchTarget SearchTarget
        {
            get => _searchTarget;
            set => SetProperty(ref _searchTarget, value);
        }

        private IllustSortMode _sortMode = IllustSortMode.Latest;
        public IllustSortMode SortMode
        {
            get => _sortMode;
            set => SetProperty(ref _sortMode, value);
        }

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

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        private AsyncEnumerableCollection<Illust>? _illusts;
        public AsyncEnumerableCollection<Illust>? Illusts
        {
            get => _illusts;
            private set => SetProperty(ref _illusts, value);
        }

        public void DoSearch()
        {
            var options = new IllustFilterOptions
            {
                SortMode = SortMode,
                MinBookmarkCount = MinBookmarkEnabled ? MinBookmark : null,
                MaxBookmarkCount = MaxBookmarkEnabled ? MaxBookmark : null,
                StartDate = StartDateEnabled ? StartDate : null,
                EndDate = EndDateEnabled ? EndDate : null
            };

            if (Tag != null)
                EffectiveWord = SearchWord;

            var query = Tag != null
                ? Tag.GetIllustsAsync(options)
                : _parent.PixivClient.SearchIllustsAsync(SearchWord, SearchTarget, options);

            Illusts = new(query.Age(Age));
        }
    }

    public class IllustSearchPageVM : ObservableObject
    {
        internal readonly PixivClient PixivClient;

        public IllustSearchPageVM(PixivSettings settings) => PixivClient = settings.Client;

        public ObservableCollection<IllustSearchVM> Tabs { get; } = new();

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        public void AddTab()
        {
            Tabs.Add(new(this));
            SelectedIndex = Tabs.Count - 1;
        }

        public void GoToTag(Tag tag)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                if (Tabs[i].Tag?.Name == tag.Name)
                {
                    SelectedIndex = i;
                    return;
                }
            }

            Tabs.Add(new(this, tag));
            SelectedIndex = Tabs.Count - 1;
        }

        public void CloseTab(IllustSearchVM tab) => Tabs.Remove(tab);
    }
}
