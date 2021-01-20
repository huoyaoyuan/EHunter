#nullable enable

using System.IO;
using Windows.Storage.Streams;

namespace EHunter.Services.ImageCaching
{
    public sealed class ImageEntry
    {
        private readonly byte[] _data;

        public ImageEntry(byte[] data) => _data = data;

        public IRandomAccessStream GetWinRTStream()
        {
            // projected stream will cause dead lock in BitmapSource.SetSource (non-Async)

            using var mms = new MemoryStream(_data);
            var inmms = new InMemoryRandomAccessStream();
            mms.CopyTo(inmms.AsStream());
            inmms.Seek(0);

            return inmms;
        }
    }
}
