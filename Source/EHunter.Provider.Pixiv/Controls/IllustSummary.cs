using Meowtrix.PixivApi.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

#nullable enable

namespace EHunter.Provider.Pixiv.Controls
{
    public class IllustSummary : Control
    {
        public IllustSummary() => DefaultStyleKey = typeof(IllustSummary);

        public static readonly DependencyProperty IllustProperty
            = DependencyProperty.Register(nameof(Illust), typeof(Illust), typeof(IllustSummary),
                new PropertyMetadata(null));
        public Illust? Illust
        {
            get => (Illust?)GetValue(IllustProperty);
            set => SetValue(IllustProperty, value);
        }
    }
}
