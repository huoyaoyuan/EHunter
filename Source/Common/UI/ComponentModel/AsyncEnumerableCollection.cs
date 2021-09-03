using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
using Windows.Foundation;

namespace EHunter.ComponentModel
{
#pragma warning disable CA2252 // TODO: https://github.com/dotnet/roslyn-analyzers/issues/5366
    internal class AsyncEnumerableCollection<T> :
        ObservableCollection<T>,
        ISupportIncrementalLoading,
        IBindableCollection<T>
#pragma warning restore CA2252
    {
        private readonly ConfiguredCancelableAsyncEnumerable<T>.Enumerator _enumerator;

        public AsyncEnumerableCollection(
            IAsyncEnumerable<T> valueEnumerable,
            CancellationToken cancellation = default)
        {
            _enumerator = valueEnumerable
                .ConfigureAwait(true)
                .WithCancellation(cancellation)
                .GetAsyncEnumerator();
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
            => LoadMoreItemsAsyncCore(count).AsAsyncOperation();

        private async Task<LoadMoreItemsResult> LoadMoreItemsAsyncCore(uint count)
        {
            uint loaded = 0;

            while (loaded < count)
            {
                try
                {
                    if (await _enumerator.MoveNextAsync())
                    {
                        Add(_enumerator.Current);
                        loaded++;
                    }
                    else
                    {
                        HasMoreItems = false;
                        await _enumerator.DisposeAsync();
                        break;
                    }
                }
                catch
                {
                    HasMoreItems = false;
                    break;
                }
            }

            return new LoadMoreItemsResult(loaded);
        }

        private bool _hasMoreItems = true;
        public bool HasMoreItems
        {
            get => _hasMoreItems;
            private set
            {
                if (_hasMoreItems != value)
                {
                    _hasMoreItems = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasMoreItems)));
                }
            }
        }
    }
}
