using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EHunter.ComponentModel
{
    public abstract partial class TabsViewModel<T> : ObservableObject,
        IActivatable
    {
        public ObservableCollection<T> Tabs { get; } = new();

        // TODO: source generation for generic
        private T? _selectedItem;
        public T? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public void AddTab()
        {
            var newTab = CreateNewTab();
            Tabs.Add(newTab);
            SelectedItem = newTab;
        }

        protected abstract T CreateNewTab();

        public void CloseTab(T item) => Tabs.Remove(item);
        public void CloseTab(object item) => CloseTab((T)item);

        public void OnActivated()
        {
            if (Tabs.Count == 0)
                AddTab();
        }
        public void OnDeactivated() { }
    }
}
