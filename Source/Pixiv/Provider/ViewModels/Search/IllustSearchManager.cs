using System.Collections.ObjectModel;
using EHunter.DependencyInjection;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.Pixiv.ViewModels.Search
{
    public class IllustSearchManager : ObservableObject
    {
        internal readonly ICustomResolver<PixivClient> ClientResolver;
        internal readonly IllustVMFactory IllustVMFactory;

        public IllustSearchManager(ICustomResolver<PixivClient> clientResolver,
            IllustVMFactory illustVMFactory)
        {
            ClientResolver = clientResolver;
            IllustVMFactory = illustVMFactory;
        }

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
