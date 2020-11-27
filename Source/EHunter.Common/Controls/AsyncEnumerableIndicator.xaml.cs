using EHunter.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#nullable enable

namespace EHunter.Controls
{
    public sealed partial class AsyncEnumerableIndicator : UserControl
    {
        public AsyncEnumerableIndicator() => InitializeComponent();

        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(nameof(Collection), typeof(IAsyncEnumerableCollection), typeof(AsyncEnumerableIndicator),
                new PropertyMetadata(null));
        public IAsyncEnumerableCollection? Collection
        {
            get => (IAsyncEnumerableCollection?)GetValue(CollectionProperty);
            set => SetValue(CollectionProperty, value);
        }
    }
}
