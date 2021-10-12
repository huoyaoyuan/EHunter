using System.Composition;
using EHunter.ComponentModel;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.User
{
    [Export]
    public class JumpToUserManager : TabsViewModel<JumpToUserVM>
    {
        private readonly PixivVMFactory _factory;

        [ImportingConstructor]
        public JumpToUserManager(PixivVMFactory factory) => _factory = factory;

        protected override JumpToUserVM CreateNewTab() => _factory.JumpToUser();

        public void GoToUser(UserInfo user)
        {
            var vm = Tabs.FirstOrDefault(x => x.UserInfo?.Id == user.Id);
            if (vm is null)
            {
                Tabs.Add(vm = _factory.JumpToUser(user));
            }

            SelectedItem = vm;
        }
    }
}
