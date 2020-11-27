using System.ComponentModel;
using Microsoft.UI.Xaml.Data;

namespace EHunter.ComponentModel
{
#pragma warning disable CA1711
    public interface IAsyncEnumerableCollection :
#pragma warning restore CA1711
        ISupportIncrementalLoading,
        INotifyPropertyChanged
    {
        int Count { get; }
    }
}
