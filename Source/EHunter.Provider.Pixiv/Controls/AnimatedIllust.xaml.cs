using System;
using Meowtrix.PixivApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable enable

namespace EHunter.Provider.Pixiv.Controls
{
    public sealed partial class AnimatedIllust : UserControl
    {
        public AnimatedIllust() => InitializeComponent();

        private Illust? _illust;
        public Illust? Illust
        {
            get => _illust;
            set
            {
                if (_illust != value)
                {
                    _illust = value;
                    LoadAnimation(_illust, false);
                }
            }
        }

        private byte[]? _bitmapData;

        private async void LoadAnimation(Illust? illust, bool refresh)
        {
            if (illust != null)
            {
                var bitmap = new BitmapImage();
                _bitmapData = null;
                image.Source = bitmap;
                loadingProgress.IsActive = true;

                try
                {
                    var cache = Ioc.Default.GetRequiredService<IMemoryCache>();
                    if (refresh)
                        cache.Remove(illust.Id);

                    byte[] data = await cache.GetOrCreateAsync(illust.Id, async entry =>
                    {
                        byte[] data = await GifHelper.ComposeGifAsync(illust).ConfigureAwait(false);

                        entry.SetSize(data.Length);
                        return data;
                    }).ConfigureAwait(true);

                    var stream = await data.CopyAsWinRTStreamAsync().ConfigureAwait(true);

                    if (image.Source == bitmap)
                    {
                        _bitmapData = data;
                        loadingProgress.IsActive = false;
                    }

                    await bitmap.SetSourceAsync(stream);
                }
                catch
                {
                    if (image.Source == bitmap)
                        loadingProgress.IsActive = false;
                }
            }
            else
            {
                image.Source = null;
                _bitmapData = null;
                loadingProgress.IsActive = false;
            }
        }
    }
}
