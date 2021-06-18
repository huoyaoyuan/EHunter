using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Illusts
{
    public class IllustPageVM
    {
        private readonly IllustPage _page;

        internal IllustPageVM(IllustVM illust, IllustPage page)
        {
            Illust = illust;
            _page = page;
        }

        public int Index => _page.Index;

        public IllustVM Illust { get; }

        public Task<Stream> RequestStreamAsync(IllustSize size, CancellationToken cancellation = default)
            => _page.AtSize(size).RequestStreamAsync(cancellation);
    }
}
