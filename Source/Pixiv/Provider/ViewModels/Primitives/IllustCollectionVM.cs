using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.ComponentModel;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Primitives
{
    public abstract class IllustCollectionVM : ObservableObject
    {
        private readonly IllustVMFactory _illustVMFactory;

        protected IllustCollectionVM(IllustVMFactory illustVMFactory) => _illustVMFactory = illustVMFactory;

        private IBindableCollection<IllustVM>? _illusts;
        public IBindableCollection<IllustVM>? Illusts
        {
            get => _illusts;
            private set => SetProperty(ref _illusts, value);
        }

        protected abstract IAsyncEnumerable<Illust>? LoadIllusts();

        public void Refresh()
        {
            Illusts = _illustVMFactory.CreateAsyncCollection(
                LoadIllusts()?.Age(SelectedAge));

            // TODO: Consider AdvancedCollectionView.Filter
            // Currently doesn't work with mignon/IsR18=true
            // 8.0.0-preview2
        }

        // TODO: https://github.com/microsoft/microsoft-ui-xaml/issues/3339

        private AgeRestriction _selectedAge = AgeRestriction.All;
        public AgeRestriction SelectedAge
        {
            get => _selectedAge;
            set
            {
                if (SetProperty(ref _selectedAge, value))
                    Refresh();
            }
        }
    }
}
