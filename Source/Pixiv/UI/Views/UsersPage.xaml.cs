using System.Collections.ObjectModel;
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
    public sealed partial class UsersPage : Page
    {
        private readonly UserVMFactory _factory = Ioc.Default.GetRequiredService<UserVMFactory>();

        public UsersPage() => InitializeComponent();

        private readonly ObservableCollection<UserVM> _vms = new();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e is
                {
                    NavigationMode: NavigationMode.New or NavigationMode.Forward,
                    Parameter: NavigateToUserMessage m
                })
            {
                var vm = _vms.FirstOrDefault(x => x.UserInfo?.Id == m.User.Id);
                if (vm is null)
                {
                    _vms.Add(vm = _factory.Create(m.User));
                }

                tabView.SelectedItem = vm;
            }
        }

        private void TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
            => _vms.Remove((UserVM)args.Item);

        private void AddTab()
        {
            // TODO: SelectedItem doesn't work properly here

            _vms.Add(_factory.Create());
            tabView.SelectedIndex = _vms.Count - 1;
        }
    }
}
