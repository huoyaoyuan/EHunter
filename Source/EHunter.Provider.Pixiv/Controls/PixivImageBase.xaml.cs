using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable enable

namespace EHunter.Provider.Pixiv.Controls
{
    public abstract partial class PixivImageBase : UserControl
    {
        public PixivImageBase() => InitializeComponent();

        private byte[]? _bitmapData;

        protected async void LoadImage<T>(T imageInfo,
            Func<T, object> getCacheKey,
            Func<T, Task<byte[]>> loadImpl,
            bool refresh)
        {
            var bitmap = new BitmapImage();
            _bitmapData = null;
            image.Source = bitmap;
            loadingProgress.IsActive = true;
            copyCommand.NotifyCanExecuteChanged();

            try
            {
                object? key = getCacheKey(imageInfo);
                var cache = Ioc.Default.GetRequiredService<IMemoryCache>();
                if (refresh)
                    cache.Remove(key);

                byte[] data = await cache.GetOrCreateAsync(key, async entry =>
                {
                    byte[] data = await loadImpl(imageInfo).ConfigureAwait(true);

                    entry.SetSize(data.Length);
                    return data;
                }).ConfigureAwait(true);

                var stream = await data.CopyAsWinRTStreamAsync().ConfigureAwait(true);

                if (image.Source == bitmap)
                {
                    _bitmapData = data;
                    loadingProgress.IsActive = false;
                    copyCommand.NotifyCanExecuteChanged();
                }

                await bitmap.SetSourceAsync(stream);
            }
            catch
            {
                if (image.Source == bitmap)
                    loadingProgress.IsActive = false;
            }
        }

        protected void ClearImage()
        {
            image.Source = null;
            _bitmapData = null;
            loadingProgress.IsActive = false;
            copyCommand.NotifyCanExecuteChanged();
        }

        protected abstract void RefreshImage();

        private async void CopyRequested()
        {
            if (_bitmapData is null)
                return;

            var dataPackage = new DataPackage();
            var stream = await _bitmapData.CopyAsWinRTStreamAsync().ConfigureAwait(true);
            dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            Clipboard.SetContent(dataPackage);
        }

#pragma warning disable CA1801 // TODO: false positive - used in xaml event handler
        private void CanCopyRequested(XamlUICommand sender, CanExecuteRequestedEventArgs args)
            => args.CanExecute = _bitmapData is not null;
    }
}
