using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EHunter.ComponentModel
{
    public abstract partial class TabsViewModel<T> : ObservableObject
    {
        public ObservableCollection<T> Tabs { get; } = new();

        private T? _selectedItem;
        public T? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        public void AddTab()
        {
            // TODO: Setting SelectedItem not working. Reunion 0.5.7

            Tabs.Add(CreateNewTab());
            SelectedIndex = Tabs.Count - 1;
        }

        protected abstract T CreateNewTab();

        public void CloseTab(T item) => Tabs.Remove(item);
        public void CloseTab(object item) => CloseTab((T)item);
    }
}
