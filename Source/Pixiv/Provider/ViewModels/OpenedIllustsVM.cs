using System.Collections.ObjectModel;
using System.Threading.Tasks;
using EHunter.DependencyInjection;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.Pixiv.ViewModels
{
    public class OpenedIllustsVM : ObservableObject
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;

        public OpenedIllustsVM(ICustomResolver<PixivClient> clientResolver)
            => _clientResolver = clientResolver;

        public ObservableCollection<IllustHolderVM> Illusts { get; } = new();

        private IllustHolderVM? _selectedIllust;
        public IllustHolderVM? SelectedIllust
        {
            get => _selectedIllust;
            set => SetProperty(ref _selectedIllust, value);
        }

        private int _selectedIndex = -1;
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

            var illust = new IllustHolderVM(id, _clientResolver.Resolve().GetIllustDetailAsync(id));
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

        public void GoToIllust(Illust illust)
        {
            for (int i = 0; i < Illusts.Count; i++)
            {
                if (Illusts[i].IllustId == illust.Id)
                {
                    SelectedIndex = i;
                    return;
                }
            }

            Illusts.Add(new IllustHolderVM(illust));
            SelectedIndex = Illusts.Count - 1;
            CanClose = true;
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
