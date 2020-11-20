#nullable enable

using Windows.Storage.Streams;

namespace EHunter.Services.ImageCaching
{
    public sealed class ImageEntry
    {
        private readonly byte[] _data;

        public ImageEntry(byte[] data) => _data = data;

#pragma warning disable CA1024
        public IRandomAccessStream GetWinRTStream() => new ClonableStream(_data);
#pragma warning restore CA1024
    }
}
