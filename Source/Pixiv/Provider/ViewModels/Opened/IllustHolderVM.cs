using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.Pixiv.ViewModels.Illusts;

namespace EHunter.Pixiv.ViewModels.Opened
{
    [ObservableProperty("IllustVM", typeof(IllustVM), IsNullable = true, IsSetterPublic = false)]
    [ObservableProperty("NotFound", typeof(bool), IsSetterPublic = false)]
    public partial class IllustHolderVM : ObservableObject
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
    }
}
