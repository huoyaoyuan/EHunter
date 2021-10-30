using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EHunter.Controls
{
    [DependencyProperty("TrackedElement", typeof(FrameworkElement), IsNullable = true, ChangedMethod = "OnTrackedElementChanged")]
    public partial class SizeTracker : ContentControl
    {
        private static void OnTrackedElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((SizeTracker)d).TrackResize((FrameworkElement?)e.OldValue, (FrameworkElement?)e.NewValue);

        private void TrackResize(FrameworkElement? oldValue, FrameworkElement? newValue)
        {
            if (oldValue is not null)
            {
                oldValue.SizeChanged -= OnSizeChanged;
            }

            if (newValue is not null)
            {
                newValue.SizeChanged += OnSizeChanged;
                Width = newValue.Width;
                Height = newValue.Height;
            }
            else
            {
                Width = double.NaN;
                Height = double.NaN;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Width = e.NewSize.Width;
            Height = e.NewSize.Height;
        }
    }
}
