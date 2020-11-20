using System.Collections.Generic;
using EHunter.Controls;
using EHunter.Provider.Pixiv.Services.ImageCaching;
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
}
