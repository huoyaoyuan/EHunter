using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace EHunter.ComponentModel
{
#pragma warning disable CA1010
    public interface IBindableCollection<T> :
#pragma warning restore CA1010
        IReadOnlyList<T>,
        IList,
        INotifyPropertyChanged,
        INotifyCollectionChanged
    {
    }
}
