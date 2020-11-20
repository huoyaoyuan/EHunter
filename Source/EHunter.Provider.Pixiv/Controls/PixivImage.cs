using System.Collections.Generic;
using System.Threading.Tasks;
using EHunter.Controls;
using EHunter.Services.ImageCaching;
using Meowtrix.PixivApi.Models;

#nullable enable

namespace EHunter.Provider.Pixiv.Controls
{
    public class PixivImage : RichImageBase
    {
        private ImageInfo? _imageInfo;
        public ImageInfo? ImageInfo
        {
            get => _imageInfo;
            set
            {
                if (!EqualityComparer<ImageInfo?>.Default.Equals(_imageInfo, value))
                {
                    _imageInfo = value;
                    SetImageEntry(PixivIllustRequest.TryCreate(value));
                }
            }
        }
    }

    internal sealed class PixivIllustRequest : ImageRequest
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
