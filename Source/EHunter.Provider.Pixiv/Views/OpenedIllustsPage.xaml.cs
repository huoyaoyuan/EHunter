using System.Linq;
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
    public sealed partial class OpenedIllustsPage : Page
    {
        public OpenedIllustsPage() => InitializeComponent();

        private readonly OpenedIllustsVM _vm = Ioc.Default.GetRequiredService<OpenedIllustsVM>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e is
                {
                    NavigationMode: NavigationMode.New or NavigationMode.Forward,
                    Parameter: NavigateToIllustMessage m
                })
            {
                if (_vm.Illusts.FirstOrDefault(x => x.Id == m.Illust.Id) is not { } i)
                {
                    _vm.Illusts.Add(i = m.Illust);
                    closeButton.IsEnabled = true;
                }

                _vm.SelectedIllust = i;
            }
        }

        private void CloseImage()
        {
            if (_vm.SelectedIndex >= 0)
            {
                int oldIndex = _vm.SelectedIndex;
                _vm.Illusts.RemoveAt(_vm.SelectedIndex);

                if (_vm.Illusts.Count == 0)
                {
                    closeButton.IsEnabled = false;
                    _vm.SelectedIndex = -1;
                    _vm.SelectedIllust = null;
                }
                else
                {
                    _vm.SelectedIndex = oldIndex == _vm.Illusts.Count
                        ? _vm.Illusts.Count - 1
                        : oldIndex;
                }
            }
        }
    }
}
