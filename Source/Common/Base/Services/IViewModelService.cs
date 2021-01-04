using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using EHunter.ComponentModel;

namespace EHunter.Services
{
    public interface IViewModelService
    {
        [return: NotNullIfNotNull("asyncEnumerable")]
        IBindableCollection<T>? CreateAsyncCollection<T>(IAsyncEnumerable<T>? asyncEnumerable);
    }
}
