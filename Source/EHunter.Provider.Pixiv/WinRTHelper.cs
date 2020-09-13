using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace EHunter.Provider.Pixiv
{
    public static class WinRTHelper
    {
        public static async ValueTask<InMemoryRandomAccessStream> CopyAsWinRTStreamAsync(this byte[] data)
        {
            // Workaround for AsRandomAccessStream not working

            var mms = new InMemoryRandomAccessStream
            {
                Size = (ulong)data.Length
            };
            var writer = new DataWriter(mms);
            writer.WriteBytes(data);
            await writer.StoreAsync();
            mms.Seek(0);
            return mms;
        }
    }
}
