using System.Windows.Input;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Illusts
{
    public class IllustTagVM
    {
        private readonly PixivVMFactory _vmFactory;

        public IllustTagVM(PixivVMFactory vmFactory, Tag tag)
        {
            _vmFactory = vmFactory;
            Tag = tag;
        }

        public Tag Tag { get; }

        public ICommand NavigateToTag => _vmFactory.NavigateToTagCommand;
    }
}
