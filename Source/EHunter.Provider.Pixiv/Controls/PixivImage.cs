using System.Collections.Generic;
using Meowtrix.PixivApi.Models;

namespace EHunter.Provider.Pixiv.Controls
{
    public class PixivImage : PixivImageBase
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
                    LoadImage(value, false);
                }
            }
        }

        private void LoadImage(ImageInfo? imageInfo, bool refresh)
        {
            if (imageInfo is { } info)
                LoadImage(info,
                    i => i.Uri,
                    async i =>
                    {
                        using var response = await i.RequestAsync().ConfigureAwait(true);
                        return await response.EnsureSuccessStatusCode()
                            .Content.ReadAsByteArrayAsync().ConfigureAwait(true);
                    },
                    refresh);
            else
                ClearImage();
        }

        protected override void RefreshImage() => LoadImage(ImageInfo, true);
    }
}
