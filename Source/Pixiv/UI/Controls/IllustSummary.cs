using EHunter.Pixiv.ViewModels.Illusts;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace EHunter.Pixiv.Controls
{
    [DependencyProperty("ViewModel", typeof(IllustVM), IsNullable = true)]
    public partial class IllustSummary : Control
    {
        public IllustSummary() => DefaultStyleKey = typeof(IllustSummary);
    }
}
