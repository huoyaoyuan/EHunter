using EHunter.Pixiv.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EHunter.Pixiv.Controls
{
    public class IllustSummary : Control
    {
        public IllustSummary() => DefaultStyleKey = typeof(IllustSummary);

        public static readonly DependencyProperty ViewModelProperty
            = DependencyProperty.Register(nameof(ViewModel), typeof(IllustVM), typeof(IllustSummary),
                new PropertyMetadata(null));
        public IllustVM? ViewModel
        {
            get => (IllustVM?)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
    }
}
