using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.Pixiv.ViewModels.User;
using Meowtrix.PixivApi;

namespace EHunter.Pixiv.ViewModels.Search
{
    [Export]
    public partial class UserSearchVM : ObservableObject
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;
        private readonly PixivVMFactory _factory;

        [ObservableProperty]
        private string _searchWord = string.Empty;

        [ObservableProperty]
        private IBindableCollection<UserWithPreviewVM>? _users;

        [ImportingConstructor]
        public UserSearchVM(ICustomResolver<PixivClient> clientResolver,
            PixivVMFactory factory)
        {
            _clientResolver = clientResolver;
            _factory = factory;
        }

        public void Search()
        {
            var client = _clientResolver.Resolve();
            var users = client.SearchUsersAsync(SearchWord);
            Users = _factory.CreateAsyncCollection(users);
        }
    }
}
