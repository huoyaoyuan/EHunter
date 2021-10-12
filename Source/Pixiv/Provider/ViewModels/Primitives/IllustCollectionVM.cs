using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.ComponentModel;
using EHunter.Pixiv.ViewModels.Illusts;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Primitives
{
    [ObservableProperty("Illusts", typeof(IBindableCollection<IllustVM>), IsNullable = true, IsSetterPublic = false)]
    [ObservableProperty("SelectedAge", typeof(AgeRestriction), Initializer = "AgeRestriction.All", ChangedAction = "Refresh();")]
    public abstract partial class IllustCollectionVM : ObservableObject
    {
        private readonly PixivVMFactory _factory;

        protected IllustCollectionVM(PixivVMFactory factory) => _factory = factory;

        protected abstract IAsyncEnumerable<Illust>? LoadIllusts();

        public void Refresh()
        {
            Illusts = _factory.CreateAsyncCollection(
                LoadIllusts()?.Age(SelectedAge));

            // TODO: Consider AdvancedCollectionView.Filter
            // Currently doesn't work with mignon/IsR18=true
            // 8.0.0-preview2
        }
    }
}
