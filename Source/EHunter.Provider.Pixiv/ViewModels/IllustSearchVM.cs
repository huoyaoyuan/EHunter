using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using EHunter.ComponentModel;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace EHunter.Provider.Pixiv.ViewModels
{
    public class EnumValueHolder<T> : ObservableObject
        where T : struct, Enum
    {
        // TODO: part of workaround of https://github.com/microsoft/microsoft-ui-xaml/issues/3339

        public EnumValueHolder()
        {
            if (Unsafe.SizeOf<T>() != sizeof(int))
                throw new NotSupportedException("This type must be used with int-sized enum.");
        }

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (SetProperty(ref _value, value))
                    OnPropertyChanged(nameof(IntValue));
            }
        }

        public int IntValue
        {
            get => Unsafe.As<T, int>(ref _value);
            set => Value = Unsafe.As<int, T>(ref value);
        }
    }

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

        public EnumValueHolder<AgeRestriction> SelectedAge { get; } = new();

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
                SortMode = SortMode.Value,
                MinBookmarkCount = MinBookmarkEnabled ? MinBookmark : null,
                MaxBookmarkCount = MaxBookmarkEnabled ? MaxBookmark : null,
                StartDate = StartDateEnabled ? StartDate.DateTime : null,
                EndDate = EndDateEnabled ? EndDate.DateTime : null
            };

            if (Tag != null)
                EffectiveWord = SearchWord;

            var query = Tag != null
                ? Tag.GetIllustsAsync(options)
                : _parent.PixivClient.SearchIllustsAsync(SearchWord, SearchTarget.Value, options);

            Illusts = new(query.Age(SelectedAge.Value));
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
