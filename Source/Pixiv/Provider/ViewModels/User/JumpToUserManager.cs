﻿using System.Composition;
using System.Linq;
using EHunter.ComponentModel;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.User
{
    [Export]
    public class JumpToUserManager : TabsViewModel<JumpToUserVM>
    {
        private readonly UserVMFactory _factory;

        [ImportingConstructor]
        public JumpToUserManager(UserVMFactory factory) => _factory = factory;

        protected override JumpToUserVM CreateNewTab() => _factory.Create();

        public void GoToUser(UserInfo user)
        {
            var vm = Tabs.FirstOrDefault(x => x.UserInfo?.Id == user.Id);
            if (vm is null)
            {
                Tabs.Add(vm = _factory.Create(user));
            }

            SelectedItem = vm;
        }
    }
}