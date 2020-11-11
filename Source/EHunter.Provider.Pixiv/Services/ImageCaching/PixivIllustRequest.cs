using System.Threading.Tasks;
using Meowtrix.PixivApi.Models;

namespace EHunter.Provider.Pixiv.Services.ImageCaching
{
    public sealed class PixivIllustRequest : ImageRequest
    {
        private readonly ImageInfo _imageInfo;

        public PixivIllustRequest(ImageInfo imageInfo) => _imageInfo = imageInfo;

        private record CacheKey(string UriString);
        public override object MemoryCacheKey => new CacheKey(_imageInfo.Uri.OriginalString);

        public override async Task<byte[]> GetImageAsync()
        {
            using var response = await _imageInfo.RequestAsync().ConfigureAwait(false);
            return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        public static PixivIllustRequest? TryCreate(ImageInfo? imageInfo)
            => imageInfo is { } info ? new(info) : null;
    }
}
