#nullable enable

using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace EHunter.Services.ImageCaching
{
    public sealed class ImageEntry
    {
        private readonly byte[] _data;

        public ImageEntry(byte[] data) => _data = data;

        public async Task<IRandomAccessStream> GetWinRTStream()
        {
            // projected stream will cause dead lock in BitmapSource.SetSource (non-Async)

            using var mms = new MemoryStream(_data);
            var inmms = new InMemoryRandomAccessStream();
            // random ObjectDisposedException in CopyTo
            // fixed in CsWinRT latest master
            await mms.CopyToAsync(inmms.AsStream()).ConfigureAwait(false);
            inmms.Seek(0);

            return inmms;
        }
    }
}
