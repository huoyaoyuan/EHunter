using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Provider.Pixiv.Views
{
    public sealed partial class PixivImage : UserControl
    {
        public PixivImage() => InitializeComponent();

        private IRandomAccessStream? _sourceStream;
        public IRandomAccessStream? SourceStream
        {
            get => _sourceStream;
            set
            {
                if (_sourceStream != value)
                {
                    _sourceStream = value;

                    if (value is null)
                        image.Source = null;
                    else
                    {
                        var bitmap = new BitmapImage();
                        bitmap.SetSource(value);
                        image.Source = bitmap;
                    }
                }
            }
        }
    }
}
