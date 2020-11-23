using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

#nullable enable

namespace EHunter.Controls
{
    public class SizeTracker : ContentControl
    {
        public static readonly DependencyProperty TrackedElementProperty
            = DependencyProperty.Register(nameof(TrackedElement), typeof(FrameworkElement), typeof(SizeTracker),
                new PropertyMetadata(null, (d, e) => ((SizeTracker)d).TrackResize((FrameworkElement?)e.OldValue, (FrameworkElement?)e.NewValue)));
        public FrameworkElement? TrackedElement
        {
            get => (FrameworkElement?)GetValue(TrackedElementProperty);
            set => SetValue(TrackedElementProperty, value);
        }

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
