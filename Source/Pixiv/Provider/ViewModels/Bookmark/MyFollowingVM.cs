using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.Pixiv.ViewModels.User;
using Meowtrix.PixivApi;

namespace EHunter.Pixiv.ViewModels.Bookmark
{
    [Export]
    public partial class MyFollowingVM : ObservableObject
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;
        private readonly PixivVMFactory _factory;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private IBindableCollection<UserWithPreviewVM>? _users;

        [ImportingConstructor]
        public MyFollowingVM(ICustomResolver<PixivClient> clientResolver,
            PixivVMFactory factory)
        {
            _clientResolver = clientResolver;
            _factory = factory;

            Refresh();
        }

        public void Refresh()
        {
            var client = _clientResolver.Resolve();
            Users = _factory.CreateAsyncCollection(client.GetMyFollowingUsersAsync());
        }
    }
}
