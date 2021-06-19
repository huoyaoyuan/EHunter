using System.IO;
using System.Threading.Tasks;

namespace EHunter.Media
{
    public interface IStorageService<in TKey> where TKey : notnull
    {
        ValueTask<bool> IsStored(TKey key);

        ValueTask<Stream?> TryGet(TKey key);

        ValueTask Store(TKey key, Stream stream);
    }
}
