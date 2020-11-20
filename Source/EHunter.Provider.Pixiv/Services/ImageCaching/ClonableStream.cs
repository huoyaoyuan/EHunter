using System.IO;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace EHunter.Provider.Pixiv.Services.ImageCaching
{
    /// <summary>
    /// Workaround for <see cref="WindowsRuntimeStreamExtensions.AsRandomAccessStream"/>
    /// not supporting <see cref="IRandomAccessStream.CloneStream"/>.
    /// </summary>
    internal class ClonableStream : IRandomAccessStream
    {
        private readonly byte[] _data;
        private readonly IRandomAccessStream _underlyingStream;

        public ClonableStream(byte[] data)
        {
            _data = data;
            _underlyingStream = new MemoryStream(data).AsRandomAccessStream();
        }
        public IRandomAccessStream CloneStream() => new ClonableStream(_data);

        #region Forwarded members
        public IInputStream GetInputStreamAt(ulong position) => _underlyingStream.GetInputStreamAt(position);
        public IOutputStream GetOutputStreamAt(ulong position) => _underlyingStream.GetOutputStreamAt(position);
        public void Seek(ulong position) => _underlyingStream.Seek(position);

        public bool CanRead => _underlyingStream.CanRead;

        public bool CanWrite => _underlyingStream.CanWrite;

        public ulong Position => _underlyingStream.Position;

        public ulong Size { get => _underlyingStream.Size; set => _underlyingStream.Size = value; }

        public IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options) => _underlyingStream.ReadAsync(buffer, count, options);
        public IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer) => _underlyingStream.WriteAsync(buffer);
        public IAsyncOperation<bool> FlushAsync() => _underlyingStream.FlushAsync();
        public void Dispose() => _underlyingStream.Dispose();
        #endregion
    }
}
