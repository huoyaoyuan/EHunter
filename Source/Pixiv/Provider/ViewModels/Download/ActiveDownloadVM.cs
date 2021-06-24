using EHunter.Media;
using EHunter.Pixiv.Services.Download;

namespace EHunter.Pixiv.ViewModels.Download
{
    public class ActiveDownloadVM
    {
        public ActiveDownloadVM(DownloadTask task, PixivVMFactory factory)
        {
            Task = task;
            Preview = factory.GetImage(task.Illust.Pages[0].Medium);
        }

        public DownloadTask Task { get; }

        public IImageSource Preview { get; }
    }
}
