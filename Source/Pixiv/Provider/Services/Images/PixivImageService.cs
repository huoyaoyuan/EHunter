using System.Composition;
using Microsoft.Extensions.Caching.Memory;

namespace EHunter.Pixiv.Services.Images
{
    [Export, Shared]
    public partial class PixivImageService
    {
        private readonly IMemoryCache _memoryCache;

        [ImportingConstructor]
        public PixivImageService(IMemoryCache memoryCache) => _memoryCache = memoryCache;
    }
}
