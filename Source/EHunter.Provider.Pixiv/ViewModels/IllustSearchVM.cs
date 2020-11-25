using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace EHunter.Provider.Pixiv.ViewModels
{
    public class IllustSearchVM : ObservableObject
    {
        private readonly IllustSearchPageVM _parent;

        internal IllustSearchVM(IllustSearchPageVM parent) => _parent = parent;
    }

    public class IllustSearchPageVM : ObservableObject
    {
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

        public void CloseTab(IllustSearchVM tab) => Tabs.Remove(tab);
    }
}
