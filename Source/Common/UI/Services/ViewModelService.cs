using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using EHunter.ComponentModel;

#nullable enable

namespace EHunter.Services
{
    public class ViewModelService : IViewModelService
    {
        [return: NotNullIfNotNull("asyncEnumerable")]
        public IBindableCollection<T>? CreateAsyncCollection<T>(IAsyncEnumerable<T>? asyncEnumerable)
            => asyncEnumerable is null ? null : new AsyncEnumerableCollection<T>(asyncEnumerable);
    }
}
