using System.ComponentModel;

namespace EHunter.ComponentModel
{
#pragma warning disable CA1711
    public interface IAsyncEnumerableCollection :
#pragma warning restore CA1711
        INotifyPropertyChanged
    {
        bool HasMoreItems { get; }
        int Count { get; }
    }
}
