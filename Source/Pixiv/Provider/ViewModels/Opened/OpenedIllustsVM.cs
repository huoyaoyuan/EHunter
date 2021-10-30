using System.Collections.ObjectModel;
using System.Composition;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.Pixiv.ViewModels.Illusts;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Opened
{
    [Export]
    public partial class OpenedIllustsVM : ObservableObject
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;
        private readonly PixivVMFactory _factory;

        [ObservableProperty]
        private IllustHolderVM? _selectedIllust;

        [ObservableProperty]
        private int _selectedIndex = -1;

        [ObservableProperty]
        private string _idToOpenText = string.Empty;

        [ObservableProperty]
        private bool _canClose;

        [ImportingConstructor]
        public OpenedIllustsVM(ICustomResolver<PixivClient> clientResolver,
            PixivVMFactory factory)
        {
            _clientResolver = clientResolver;
            _factory = factory;
        }

        public ObservableCollection<IllustHolderVM> Illusts { get; } = new();

        public void OpenFromId()
        {
            if (!int.TryParse(IdToOpenText, out int id))
                return;

            IdToOpenText = string.Empty;

            async Task<IllustVM> GetIllustAsync(int id)
                => _factory.CreateViewModel(await _clientResolver.Resolve()
                    .GetIllustDetailAsync(id)
                    .ConfigureAwait(true));

            var illust = new IllustHolderVM(id, GetIllustAsync(id));
            Illusts.Add(illust);
            SelectedIllust = illust;
            CanClose = true;
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

            Illusts.Add(new(_factory.CreateViewModel(illust)));
            SelectedIndex = Illusts.Count - 1;
            CanClose = true;
        }
    }
}
