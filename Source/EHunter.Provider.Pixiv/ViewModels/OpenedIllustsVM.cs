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

        public ObservableCollection<IllustHolderVM> Illusts { get; } = new();

        private IllustHolderVM? _selectedIllust;
        public IllustHolderVM? SelectedIllust
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

        public void OpenFromId()
        {
            if (!int.TryParse(IdToOpenText, out int id))
                return;

            IdToOpenText = string.Empty;

            var illust = new IllustHolderVM(id, _client.GetIllustDetailAsync(id));
            Illusts.Add(illust);
            SelectedIllust = illust;
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
            if (Illusts.FirstOrDefault(x => x.IllustId == illust.Id) is not { } i)
            {
                Illusts.Add(i = new IllustHolderVM(illust));
                CanClose = true;
            }

            SelectedIllust = i;
        }
    }

    public class IllustHolderVM : ObservableObject
    {
        public IllustHolderVM(int illustId, Task<Illust> task)
        {
            IllustId = illustId;
            InitIllustAsync(task);

            async void InitIllustAsync(Task<Illust> task)
            {
                try
                {
                    Illust = await task.ConfigureAwait(true);
                }
                catch
                {
                    NotFound = true;
                }
            }
        }

        public IllustHolderVM(Illust illust)
        {
            IllustId = illust.Id;
            Illust = illust;
        }

        public int IllustId { get; }

        private Illust? _illust;
        public Illust? Illust
        {
            get => _illust;
            set => SetProperty(ref _illust, value);
        }

        private bool _notFound;
        public bool NotFound
        {
            get => _notFound;
            set => SetProperty(ref _notFound, value);
        }
    }
}
