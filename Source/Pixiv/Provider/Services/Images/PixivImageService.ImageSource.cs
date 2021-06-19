using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EHunter.Media;
using Meowtrix.PixivApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.Pixiv.Services.Images
{
    public partial class PixivImageService
    {
        private class PixivImageSource : PixivImageSourceBase
        {
            private readonly ImageInfo _imageInfo;

            public PixivImageSource(PixivImageService owner, ImageInfo imageInfo)
                : base(owner) => _imageInfo = imageInfo;

            protected override object CreateCacheKey() => new PixivCacheKey(_imageInfo.Uri);

            protected override async Task<byte[]> RequestAsync(CancellationToken cancellationToken)
            {
                var response = await _imageInfo.RequestAsync(cancellationToken).ConfigureAwait(false);
                return await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private record PixivCacheKey(Uri Uri);

        private class PixivAnimatedImageSource : PixivImageSourceBase
        {
            private readonly Illust _illust;

            public PixivAnimatedImageSource(PixivImageService owner, Illust illust)
                : base(owner) => _illust = illust;

            protected override object CreateCacheKey() => new AnimatedCacheKey(_illust.Id);

            protected override async Task<byte[]> RequestAsync(CancellationToken cancellationToken)
            {
                var details = await _illust.GetAnimatedDetailAsync(cancellationToken).ConfigureAwait(false);
                var mms = new MemoryStream();
                await GifHelper.ComposeGifAsync(details, mms, cancellationToken).ConfigureAwait(false);
                mms.Seek(0, SeekOrigin.Begin);

                byte[] array = new byte[mms.Length];
                mms.Read(array);
                return array;
            }
        }

        private record AnimatedCacheKey(int IllustId);

        private abstract class PixivImageSourceBase : IImageSource
        {
            private readonly PixivImageService _owner;
            private byte[]? _data;

            protected PixivImageSourceBase(PixivImageService owner)
                => _owner = owner;

            protected abstract object CreateCacheKey();

            protected abstract Task<byte[]> RequestAsync(CancellationToken cancellationToken);

            public ValueTask<Stream> GetImageAsync(bool refresh = false, CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!refresh && _data != null)
                    return new(new MemoryStream(_data));

                object? cacheKey = CreateCacheKey();
                if (!refresh)
                {
                    if (_owner._memoryCache.TryGetValue(cacheKey, out _data))
                        return new(new MemoryStream(_data!));
                }
                else
                {
                    _data = null;
                    _owner._memoryCache.Remove(cacheKey);
                }

                return GetAsync(cacheKey, cancellationToken);

                async ValueTask<Stream> GetAsync(object cacheKey, CancellationToken cancellationToken)
                {
                    _data = await RequestAsync(cancellationToken).ConfigureAwait(false);
                    _owner._memoryCache.Set(cacheKey, _data);
                    return new MemoryStream(_data);
                }
            }
        }
    }
}
