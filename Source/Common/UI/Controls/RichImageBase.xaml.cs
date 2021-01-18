using System;
using EHunter.Services.ImageCaching;
using Microsoft.Toolkit.Mvvm.ComponentModel;
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
        protected void SetImageEntry(ImageRequest? request)
        {
            _bindingHelper.Holder = request is null
                ? null
                : new(request, _imageCache, copyCommand);
        }

        private void CanCopyRequested(XamlUICommand sender, CanExecuteRequestedEventArgs args)
            => args.CanExecute = _bindingHelper.Holder?.CanCopy ?? false;

        private readonly BindingHelper _bindingHelper = new();

        private class BindingHelper : ObservableObject
        {
            private ImageResultHolder? _holder;
            public ImageResultHolder? Holder
            {
                get => _holder;
                set => SetProperty(ref _holder, value);
            }
        }
        private class ImageResultHolder : ObservableObject
        {
            private readonly ImageRequest _request;
            private readonly ImageCacheService _service;
            private readonly XamlUICommand _copyCommand;
            private ImageEntry? _imageEntry;

            public ImageResultHolder(ImageRequest request, ImageCacheService service, XamlUICommand uiCommand)
            {
                _request = request;
                _service = service;
                _copyCommand = uiCommand;
                LoadAsync(false);
            }

            public async void LoadAsync(bool refresh)
            {
                _imageEntry = null;
                _copyCommand.NotifyCanExecuteChanged();

                IsLoading = true;
                LoadFailed = false;
                Source = null;

                try
                {
                    _imageEntry = await _service.GetImageAsync(_request, refresh).ConfigureAwait(true);
                }
                catch
                {
                    IsLoading = false;
                    LoadFailed = true;
                    return;
                }
                Source = new();
                await Source.SetSourceAsync(_imageEntry.GetWinRTStream());
                IsLoading = false;
                _copyCommand.NotifyCanExecuteChanged();
            }

            public void Refresh() => LoadAsync(true);

            private BitmapImage? _source;
            public BitmapImage? Source
            {
                get => _source;
                private set => SetProperty(ref _source, value);
            }

            private bool _loadFailed;
            public bool LoadFailed
            {
                get => _loadFailed;
                private set => SetProperty(ref _loadFailed, value);
            }

            private bool _isLoading;
            public bool IsLoading
            {
                get => _isLoading;
                private set => SetProperty(ref _isLoading, value);
            }

            public bool CanCopy => !IsLoading && !LoadFailed;

            public void Copy()
            {
                if (_imageEntry is null)
                    return;

                var dataPackage = new DataPackage();
                var stream = _imageEntry.GetWinRTStream();
                dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
                dataPackage.RequestedOperation = DataPackageOperation.Copy;
                Clipboard.SetContent(dataPackage);
            }
        }
    }

}
