using System.Collections.ObjectModel;
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

        private AsyncEnumerableCollection<Illust>? _illusts;
        public AsyncEnumerableCollection<Illust>? Illusts
        {
            get => _illusts;
            private set => SetProperty(ref _illusts, value);
        }

        public void DoSearch()
        {
            EffectiveWord = SearchWord;
            Illusts = new(_parent.PixivClient.SearchIllustsAsync(SearchWord, SearchTarget, new IllustFilterOptions
            {
                SortMode = SortMode
            }));
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
