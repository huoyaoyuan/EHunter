using EHunter.ComponentModel;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable enable

namespace EHunter.Controls
{
    public sealed partial class AsyncEnumerableIndicator : UserControl
    {
        public AsyncEnumerableIndicator() => InitializeComponent();

        private IAsyncEnumerableCollection? _collection;
        public IAsyncEnumerableCollection? Collection
        {
            get => _collection;
            set
            {
                _collection = value;
                Bindings.Update();
            }
        }
    }
}
