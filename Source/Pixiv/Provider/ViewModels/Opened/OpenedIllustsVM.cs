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
    [ObservableProperty("SelectedIllust", typeof(IllustHolderVM), IsNullable = true)]
    [ObservableProperty("SelectedIndex", typeof(int), Initializer = "-1")]
    [ObservableProperty("IdToOpenText", typeof(string), Initializer = "string.Empty")]
    [ObservableProperty("CanClose", typeof(bool), IsSetterPublic = false)]
    public partial class OpenedIllustsVM : ObservableObject
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;
        private readonly IllustVMFactory _illustVMFactory;

        [ImportingConstructor]
        public OpenedIllustsVM(ICustomResolver<PixivClient> clientResolver,
            IllustVMFactory illustVMFactory)
        {
            _clientResolver = clientResolver;
            _illustVMFactory = illustVMFactory;
        }

        public ObservableCollection<IllustHolderVM> Illusts { get; } = new();

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
