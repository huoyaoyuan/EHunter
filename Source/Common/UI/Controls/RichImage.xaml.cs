using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.Media;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Controls
{
    public sealed partial class RichImage : UserControl
    {
        public RichImage() => InitializeComponent();

        private IImageSource? _source;
        public IImageSource? Source
        {
            get => _source;
            set
            {
                _source = value;
                _holder = value is null
                    ? null
                    : new(value, copyCommand);
                Bindings.Update();
            }
        }

        private void CanCopyRequested(XamlUICommand sender, CanExecuteRequestedEventArgs args)
            => args.CanExecute = _holder?.CanCopy ?? false;

        private ImageResultHolder? _holder;

        private class ImageResultHolder : ObservableObject
        {
            private readonly IImageSource _imageSource;
            private readonly XamlUICommand _copyCommand;

            public ImageResultHolder(IImageSource imageSource, XamlUICommand uiCommand)
            {
                _imageSource = imageSource;
                _copyCommand = uiCommand;
                LoadAsync(false);
            }

            private async void LoadAsync(bool refresh)
            {
                _copyCommand.NotifyCanExecuteChanged();

                IsLoading = true;
                LoadFailed = false;
                Source = null;

                try
                {
                    using var stream = await _imageSource.GetImageAsync(refresh).ConfigureAwait(true);

                    var source = new BitmapImage();
                    await source.SetSourceAsync(stream.AsRandomAccessStream());

                    Source = source;
                    LoadFailed = false;
                }
                catch
                {
                    LoadFailed = true;
                }
                finally
                {

                    IsLoading = false;
                    _copyCommand.NotifyCanExecuteChanged();
                }
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

            public async void Copy()
            {
                try
                {
                    var dataPackage = new DataPackage();

                    using var stream = await _imageSource.GetImageAsync().ConfigureAwait(true);
                    var winrtStream = new InMemoryRandomAccessStream(); // don't dispose - used by clipboard
                    stream.CopyTo(winrtStream.AsStream());
                    // requires CloneStream, only supported by InMemoryRandomAccessStream
                    dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromStream(winrtStream));

                    dataPackage.RequestedOperation = DataPackageOperation.Copy;
                    Clipboard.SetContent(dataPackage);
                }
                catch
                {
                }
            }
        }
    }
}
