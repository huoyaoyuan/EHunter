﻿using System;
using EHunter.Services.ImageCaching;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable enable

namespace EHunter.Controls
{
    public abstract partial class RichImageBase : UserControl
    {
        protected RichImageBase() => InitializeComponent();

        private readonly ImageCacheService _imageCache = Ioc.Default.GetRequiredService<ImageCacheService>();
        private ImageRequest? _currentRequest;
        private ImageEntry? _currentEntry;

        protected async void SetImageEntry(ImageRequest? request, bool refreshMemoryCache = false)
        {
            _currentRequest = request;
            _currentEntry = null;

            if (request is null)
            {
                image.Source = null;
                loadingProgress.IsActive = false;
                copyCommand.NotifyCanExecuteChanged();
                return;
            }

            var bitmap = new BitmapImage();
            image.Source = bitmap;
            loadingProgress.IsActive = true;
            copyCommand.NotifyCanExecuteChanged();

            try
            {
                var entry = await _imageCache.GetImageAsync(request, refreshMemoryCache).ConfigureAwait(true);

                var stream = entry.GetWinRTStream();

                if (image.Source == bitmap)
                {
                    _currentEntry = entry;
                    loadingProgress.IsActive = false;
                    copyCommand.NotifyCanExecuteChanged();

                    await bitmap.SetSourceAsync(stream);
                }
            }
            catch
            {
                if (image.Source == bitmap)
                    loadingProgress.IsActive = false;
            }
        }

        private void RefreshImage() => SetImageEntry(_currentRequest, true);

        private void CopyRequested()
        {
            if (_currentEntry is null)
                return;

            var dataPackage = new DataPackage();
            var stream = _currentEntry.GetWinRTStream();
            dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            Clipboard.SetContent(dataPackage);
        }

        private void CanCopyRequested(XamlUICommand sender, CanExecuteRequestedEventArgs args)
            => args.CanExecute = _currentEntry is not null;
    }
}