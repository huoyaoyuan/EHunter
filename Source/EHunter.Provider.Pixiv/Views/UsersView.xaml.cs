using System.Collections.ObjectModel;
using EHunter.Provider.Pixiv.Messages;
using EHunter.Provider.Pixiv.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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
    public sealed partial class UsersView : Page
    {
        private readonly UserVMFactory _factory = Ioc.Default.GetRequiredService<UserVMFactory>();

        public UsersView() => InitializeComponent();

        private readonly ObservableCollection<UserVM> _vms
            = new ObservableCollection<UserVM>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e is
            {
                NavigationMode: NavigationMode.New or NavigationMode.Forward,
                Parameter: NavigateToUserMessage m
            })
            {
                // TODO: SelectedItem doesn't work. Only SelectedIndex + TabItemsSource work.

                //var vm = tabView.TabItems.Cast<UserSearchVM>().FirstOrDefault(x => x.UserInfo?.Id == m.User.Id);
                //if (vm is null)
                //{
                //    tabView.TabItems.Add(vm = new UserSearchVM { UserInfo = m.User });
                //}

                //tabView.SelectedItem = vm;

                int index;
                for (index = 0; index < _vms.Count; index++)
                    if (_vms[index].UserInfo?.Id == m.User.Id)
                        break;

                if (index == _vms.Count)
                    _vms.Add(_factory.Create(m.User));

                tabView.SelectedIndex = index;
            }
        }

#pragma warning disable CA1801 // TODO: false positive - used in xaml event handler

        private void TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
            => _vms.Remove((UserVM)args.Item);

        private void AddTab()
        {
            _vms.Add(_factory.Create());
            tabView.SelectedIndex = _vms.Count - 1;
        }
    }
}
