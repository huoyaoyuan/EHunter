using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EHunter.Provider.Pixiv.Services
{
    public static class AsyncEnumerableExtensions
    {
        public static async ValueTask ConsumeAsync<T>(this IAsyncEnumerable<T> source,
            CancellationToken cancellationToken = default)
        {
            await foreach (var _ in source.ConfigureAwait(false).WithCancellation(cancellationToken))
            {
            }
        }
    }
}
