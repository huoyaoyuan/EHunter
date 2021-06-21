using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EHunter.Media
{
    public interface IImageSource
    {
        ValueTask<Stream> GetImageAsync(bool refresh = false, CancellationToken cancellationToken = default);
    }
}
