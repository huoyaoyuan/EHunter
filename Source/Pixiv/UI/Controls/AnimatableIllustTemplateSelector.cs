using Meowtrix.PixivApi.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

#nullable enable

namespace EHunter.Pixiv.Controls
{
    public sealed class AnimatableIllustTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? AnimatedTemplate { get; set; }
        public DataTemplate? NonAnimatedTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
        protected override DataTemplate? SelectTemplateCore(object item)
            => item switch
            {
                null => null,
                Illust { IsAnimated: true } => AnimatedTemplate,
                _ => NonAnimatedTemplate
            };
    }
}
