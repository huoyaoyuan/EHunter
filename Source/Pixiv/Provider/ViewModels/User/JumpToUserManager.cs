using CommunityToolkit.Mvvm.Messaging;
using EHunter.ComponentModel;
using EHunter.Pixiv.Messages;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.User
{
    public sealed class JumpToUserManager : TabsViewModel<JumpToUserVM>, IRecipient<NavigateToUserMessage>
    {
        private readonly PixivVMFactory _factory;

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

        void IRecipient<NavigateToUserMessage>.Receive(NavigateToUserMessage message) => GoToUser(message.User);
    }
}
