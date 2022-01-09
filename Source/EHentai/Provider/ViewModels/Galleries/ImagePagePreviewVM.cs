using EHunter.EHentai.Api.Models;
using EHunter.Media;

namespace EHunter.EHentai.ViewModels.Galleries
{
    public class ImagePagePreviewVM
    {
        public ImagePagePreviewVM(ImagePage image, EHentaiVMFactory factory)
            => Image = factory.GetImagePreview(image);

        public IImageSource Image { get; }
    }
}
