using EHunter.Pixiv.ViewModels.Ranking;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Pixiv.Views.Ranking
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RankingPage : Page
    {
        private readonly RankingVM _vm = Ioc.Default.GetRequiredService<RankingVM>();

        public RankingPage() => InitializeComponent();
    }
}
