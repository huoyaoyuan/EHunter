using Meowtrix.PixivApi.Models;

#nullable enable

namespace EHunter.Provider.Pixiv.Controls
{
    public class AnimatedIllust : PixivImageBase
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
                    LoadImage(_illust, false);
                }
            }
        }

        private void LoadImage(Illust? illust, bool refresh)
        {
            if (illust != null)
                LoadImage(
                    illust,
                    i => i.Id,
                    i => GifHelper.ComposeGifAsync(i).AsTask(),
                    refresh);
            else
                ClearImage();
        }

        protected override void RefreshImage() => LoadImage(Illust, true);
    }
}
