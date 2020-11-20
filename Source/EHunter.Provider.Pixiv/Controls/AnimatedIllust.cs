using EHunter.Controls;
using EHunter.Provider.Pixiv.Services.ImageCaching;
using Meowtrix.PixivApi.Models;

#nullable enable

namespace EHunter.Provider.Pixiv.Controls
{
    public class AnimatedIllust : RichImageBase
    {
        private Illust? _illust;
        public Illust? Illust
        {
            get => _illust;
            set
            {
                if (_illust != value)
                {
                    _illust = value;
                    SetImageEntry(PixivAnimationRequest.TryCreate(value));
                }
            }
        }
    }
}
