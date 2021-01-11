using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.Pixiv.ViewModels.Opened
{
    public class IllustHolderVM : ObservableObject
    {
        public IllustHolderVM(int illustId, Task<IllustVM> task)
        {
            IllustId = illustId;
            InitIllustAsync(task);

            async void InitIllustAsync(Task<IllustVM> task)
            {
                try
                {
                    IllustVM = await task.ConfigureAwait(true);
                }
                catch
                {
                    NotFound = true;
                }
            }
        }

        public IllustHolderVM(IllustVM vm)
        {
            IllustId = vm.Illust.Id;
            IllustVM = vm;
        }

        public int IllustId { get; }

        private IllustVM? _illustVM;
        public IllustVM? IllustVM
        {
            get => _illustVM;
            set => SetProperty(ref _illustVM, value);
        }

        private bool _notFound;
        public bool NotFound
        {
            get => _notFound;
            set => SetProperty(ref _notFound, value);
        }
    }
}
