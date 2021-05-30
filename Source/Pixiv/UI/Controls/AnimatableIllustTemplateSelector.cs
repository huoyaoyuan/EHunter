using System;
using EHunter.Pixiv.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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
                IllustVM { Illust: { IsAnimated: true } } => AnimatedTemplate,
                IllustVM { Illust: { IsAnimated: false } } => NonAnimatedTemplate,
                _ => throw new InvalidOperationException($"{nameof(AnimatableIllustTemplateSelector)} must be used with correct type.")
            };
    }
}
