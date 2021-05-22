using System.Threading.Tasks;
using EHunter.Controls;
using EHunter.EHentai.Api.Models;
using EHunter.Services.ImageCaching;

#nullable enable

namespace EHunter.EHentai.Controls
{
    public class Thumbnail : RichImageBase
    {
        private Gallery? _gallery;
        public Gallery? Gallery
        {
            get => _gallery;
            set
            {
                if (_gallery != value)
                {
                    _gallery = value;
                    SetImageEntry(value is null ? null : new ThumbnailRequest(value));
                }
            }
        }
    }

    internal sealed class ThumbnailRequest : ImageRequest
    {
        private readonly Gallery _gallery;

        public ThumbnailRequest(Gallery gallery) => _gallery = gallery;

        public override object? MemoryCacheKey => null;

        public override Task<byte[]> GetImageAsync() => _gallery.GetImageAsync();
    }
}
