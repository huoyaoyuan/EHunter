using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using EHunter.ComponentModel;

namespace EHunter.Services
{
    public interface IViewModelService
    {
        [return: NotNullIfNotNull("asyncEnumerable")]
        IBindableCollection<T>? CreateAsyncCollection<T>(IAsyncEnumerable<T>? asyncEnumerable);

        Task<string?> BrowseFolderAsync();
    }
}
