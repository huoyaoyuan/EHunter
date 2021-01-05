using EHunter.Pixiv.ViewModels;
using Meowtrix.PixivApi.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

#nullable enable

namespace EHunter.Pixiv.Controls
{
    public class IllustSummary : Control
    {
        public IllustSummary() => DefaultStyleKey = typeof(IllustSummary);

        public static readonly DependencyProperty ViewModelProperty
            = DependencyProperty.Register(nameof(ViewModel), typeof(Illust), typeof(IllustSummary),
                new PropertyMetadata(null));
        public IllustVM? ViewModel
        {
            get => (IllustVM?)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
    }
}
