using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;

namespace EHunter.UI.ViewModels
{
    internal class MainWindowVM : ObservableObject
    {
        public ObservableCollection<TabRootVM> Tabs { get; } = new ObservableCollection<TabRootVM>
        {
            new TabRootVM()
        };

        public void AddNewTab() => Tabs.Add(new TabRootVM());

        public void CloseTab(TabView _, TabViewTabCloseRequestedEventArgs args)
            => Tabs.Remove((TabRootVM)args.Item);
    }
}
