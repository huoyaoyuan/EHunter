using System.Composition;
using EHunter.ComponentModel;

namespace EHunter.Pixiv.ViewModels.User
{
    [Export]
    public class JumpToUserManager : TabsViewModel<JumpToUserVM>
    {
        private readonly UserVMFactory _factory;

        [ImportingConstructor]
        public JumpToUserManager(UserVMFactory factory) => _factory = factory;

        protected override JumpToUserVM CreateNewTab() => _factory.Create();
    }
}
