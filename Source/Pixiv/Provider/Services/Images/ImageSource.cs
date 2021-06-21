using EHunter.Media;

namespace EHunter.Pixiv.Services.Images
{
    public abstract class ImageSource : CachedImageSource
    {
        protected ImageSource(PixivImageService owner)
            : base(owner.MemoryCache)
        {
        }
    }
}
