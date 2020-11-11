#nullable enable

using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace EHunter.Provider.Pixiv.Services.ImageCaching
{
    public sealed class ImageEntry
    {
        private readonly byte[] _data;

        public ImageEntry(byte[] data) => _data = data;

        public async ValueTask<InMemoryRandomAccessStream> GetWinRTStreamAsync()
        {
            // Workaround for AsRandomAccessStream not working

            var mms = new InMemoryRandomAccessStream
            {
                Size = (ulong)_data.Length
            };
            var writer = new DataWriter(mms);
            writer.WriteBytes(_data);
            await writer.StoreAsync();
            mms.Seek(0);
            return mms;
        }
    }
}
