using Meowtrix.PixivApi.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

#nullable enable

namespace EHunter.Provider.Pixiv.Controls
{
    public sealed class IllustFlipTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? AnimatedTemplate { get; set; }
        public DataTemplate? NonAnimatedTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
        protected override DataTemplate? SelectTemplateCore(object item)
            => item is Illust { IsAnimated: true }
            ? AnimatedTemplate
            : NonAnimatedTemplate;
    }
}
