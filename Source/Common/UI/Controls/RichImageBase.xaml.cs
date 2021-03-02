using System.IO;
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
            _holder = request is null
                ? null
                : new(request, _imageCache, copyCommand);
            Bindings.Update();
        }

        private void CanCopyRequested(XamlUICommand sender, CanExecuteRequestedEventArgs args)
            => args.CanExecute = _holder?.CanCopy ?? false;

        private ImageResultHolder? _holder;

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

                var source = new BitmapImage();

                // https://github.com/microsoft/CsWinRT/issues/682
                // only WinRT stream with SetSource can avoid random exception
                using (var stream = _imageEntry.GetStream())
                using (var winrtStream = new InMemoryRandomAccessStream())
                {
                    stream.CopyTo(winrtStream.AsStream());
                    winrtStream.Seek(0);
                    source.SetSource(winrtStream);
                }

                Source = source;
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

                using var stream = _imageEntry.GetStream();
                var winrtStream = new InMemoryRandomAccessStream(); // don't dispose - used by clipboard
                stream.CopyTo(winrtStream.AsStream());
                // requires CloneStream, only supported by InMemoryRandomAccessStream
                dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromStream(winrtStream));

                dataPackage.RequestedOperation = DataPackageOperation.Copy;
                Clipboard.SetContent(dataPackage);
            }
        }
    }

}
