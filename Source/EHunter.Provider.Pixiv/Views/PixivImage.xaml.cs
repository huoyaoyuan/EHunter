using System;
using System.Collections.Generic;
using Meowtrix.PixivApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Provider.Pixiv.Views
{
    public sealed partial class PixivImage : UserControl
    {
        public PixivImage() => InitializeComponent();

        private ImageInfo? _imageInfo;
        public ImageInfo? ImageInfo
        {
            get => _imageInfo;
            set
            {
                static async void GetImage(BitmapImage bitmap, ImageInfo info)
                {
                    try
                    {
                        var cache = Ioc.Default.GetRequiredService<IMemoryCache>();
                        byte[] data = await cache.GetOrCreateAsync(info.Uri, async entry =>
                        {
                            using var response = await info.RequestAsync().ConfigureAwait(true);
                            byte[] data = await response.EnsureSuccessStatusCode()
                                .Content.ReadAsByteArrayAsync().ConfigureAwait(true);

                            entry.SetSize(data.Length);
                            return data;
                        }).ConfigureAwait(true);

                        await bitmap.SetSourceAsync(await data.CopyAsWinRTStreamAsync().ConfigureAwait(true));
                    }
                    catch
                    {
                    }
                }

                if (!EqualityComparer<ImageInfo?>.Default.Equals(_imageInfo, value))
                {
                    _imageInfo = value;
                    if (value is { } i)
                    {
                        var bitmap = new BitmapImage();
                        image.Source = bitmap;
                        GetImage(bitmap, i);
                    }
                    else
                    {
                        image.Source = null;
                    }
                }
            }
        }
    }
}
