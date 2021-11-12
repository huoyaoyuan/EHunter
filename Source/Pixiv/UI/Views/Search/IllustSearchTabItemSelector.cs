using EHunter.Controls;
using EHunter.Pixiv.ViewModels.Search;

namespace EHunter.Pixiv.Views.Search
{
    internal class IllustSearchTabItemSelector : SwitchTemplateSelector
    {
        protected override object? MapValue(object? value)
            => ((IllustSearchVM?)value)?.Tag != null;
    }
}
