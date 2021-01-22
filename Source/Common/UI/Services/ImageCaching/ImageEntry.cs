using System.IO;

#nullable enable

namespace EHunter.Services.ImageCaching
{
    public sealed class ImageEntry
    {
        private readonly byte[] _data;

        public ImageEntry(byte[] data) => _data = data;

#pragma warning disable CA1024
        public Stream GetStream() => new MemoryStream(_data);
#pragma warning restore CA1024
    }
}
