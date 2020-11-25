using EHunter.Provider.Pixiv.Messages;
using EHunter.Provider.Pixiv.ViewModels;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EHunter.Provider.Pixiv.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IllustSearchPage : Page
    {
        private readonly IllustSearchPageVM _vm = Ioc.Default.GetRequiredService<IllustSearchPageVM>();

        public IllustSearchPage() => InitializeComponent();

        private void TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
            => _vm.CloseTab((IllustSearchVM)args.Item);

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e is
                {
                    NavigationMode: NavigationMode.New or NavigationMode.Forward,
                    Parameter: NavigateToTagMessage m
                })
            {
                _vm.GoToTag(m.Tag);
            }
        }
    }
}
