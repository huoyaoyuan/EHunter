using System.Composition;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.Pixiv.Services.Images
{
    [Export, Shared]
    public class PixivImageService
    {
        internal readonly IMemoryCache MemoryCache;

        [ImportingConstructor]
        public PixivImageService(IMemoryCache memoryCache) => MemoryCache = memoryCache;
    }
}
