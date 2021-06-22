using EHunter.Media;

namespace EHunter.Pixiv.ViewModels.Download
{
    public class ActiveDownloadVM
    {
        public ActiveDownloadVM(IllustDownloadVM task, PixivVMFactory factory)
        {
            Task = task;
            Preview = factory.GetImage(task.Illust.Pages[0].Medium);
        }

        public IllustDownloadVM Task { get; }

        public IImageSource Preview { get; }
    }
}
