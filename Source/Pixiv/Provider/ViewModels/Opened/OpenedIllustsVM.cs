using System.Collections.ObjectModel;
using System.Threading.Tasks;
using EHunter.DependencyInjection;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.Pixiv.ViewModels.Opened
{
    public class OpenedIllustsVM : ObservableObject
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;
        private readonly IllustVMFactory _illustVMFactory;

        public OpenedIllustsVM(ICustomResolver<PixivClient> clientResolver,
            IllustVMFactory illustVMFactory)
        {
            _clientResolver = clientResolver;
            _illustVMFactory = illustVMFactory;
        }

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

            async Task<IllustVM> GetIllustAsync(int id)
                => _illustVMFactory.CreateViewModel(await _clientResolver.Resolve()
                    .GetIllustDetailAsync(id)
                    .ConfigureAwait(true));

            var illust = new IllustHolderVM(id, GetIllustAsync(id));
            Illusts.Add(illust);
            SelectedIllust = illust;
            CanClose = true;
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

            Illusts.Add(new(_illustVMFactory.CreateViewModel(illust)));
            SelectedIndex = Illusts.Count - 1;
            CanClose = true;
        }
    }
}
