using System.Collections.ObjectModel;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace EHunter.Provider.Pixiv.ViewModels
{
    public class OpenedIllustsVM : ObservableObject
    {
        public ObservableCollection<Illust> Illusts { get; } = new();

        private Illust? _selectedIllust;
        public Illust? SelectedIllust
        {
            get => _selectedIllust;
            set => SetProperty(ref _selectedIllust, value);
        }
    }
}
