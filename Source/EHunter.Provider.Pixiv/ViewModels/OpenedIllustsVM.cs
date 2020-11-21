using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace EHunter.Provider.Pixiv.ViewModels
{
    public class OpenedIllustsVM : ObservableObject
    {
        private readonly PixivClient _client;

        public OpenedIllustsVM(PixivSettings settings)
            => _client = settings.Client;

        public ObservableCollection<Illust> Illusts { get; } = new();

        private Illust? _selectedIllust;
        public Illust? SelectedIllust
        {
            get => _selectedIllust;
            set => SetProperty(ref _selectedIllust, value);
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        private string _idToOpenText = string.Empty;
        public string IdToOpenText
        {
            get => _idToOpenText;
            set => SetProperty(ref _idToOpenText, value);
        }

        public async void OpenFromId()
        {
            if (!int.TryParse(IdToOpenText, out int id))
                return;

            IdToOpenText = string.Empty;

            try
            {
                var illust = await _client.GetIllustDetailAsync(id).ConfigureAwait(true);
                Illusts.Add(illust);
                SelectedIllust = illust;
            }
            catch
            {
            }
        }

        private bool _canClose;
        public bool CanClose
        {
            get => _canClose;
            private set => SetProperty(ref _canClose, value);
        }

        public void CloseCurrent()
        {
            if (SelectedIndex >= 0)
            {
                int oldIndex = SelectedIndex;
                Illusts.RemoveAt(SelectedIndex);

                if (Illusts.Count == 0)
                {
                    CanClose = false;
                    SelectedIndex = -1;
                    SelectedIllust = null;
                }
                else
                {
                    SelectedIndex = oldIndex == Illusts.Count
                        ? Illusts.Count - 1
                        : oldIndex;
                }
            }
        }

        public void AddIllust(Illust illust)
        {
            if (Illusts.FirstOrDefault(x => x.Id == illust.Id) is not { } i)
            {
                Illusts.Add(i = illust);
                CanClose = true;
            }

            SelectedIllust = i;
        }
    }
}
