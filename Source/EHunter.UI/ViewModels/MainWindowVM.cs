using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;

namespace EHunter.UI.ViewModels
{
    public sealed class MainWindowVM : ObservableObject
    {
        public ObservableCollection<TabRootVM> Tabs { get; } = new ObservableCollection<TabRootVM>
        {
            new TabRootVM()
        };


        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        public void AddNewTab()
        {
            Tabs.Add(new TabRootVM());
            SelectedIndex = Tabs.Count - 1;
        }

        public void CloseTab(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            _ = Tabs.Remove((TabRootVM)args.Item);
            if (SelectedIndex >= Tabs.Count)
                SelectedIndex = Tabs.Count - 1;
        }
    }
}
