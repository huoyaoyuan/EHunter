using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.Pixiv.ViewModels.User;
using Meowtrix.PixivApi;

namespace EHunter.Pixiv.ViewModels.Search
{
    [Export]
    [ObservableProperty("SearchWord", typeof(string), Initializer = "string.Empty")]
    [ObservableProperty("Users", typeof(IBindableCollection<UserWithPreviewVM>), IsNullable = true, IsSetterPublic = false)]
    public partial class UserSearchVM : ObservableObject
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;
        private readonly UserVMFactory _factory;

        [ImportingConstructor]
        public UserSearchVM(ICustomResolver<PixivClient> clientResolver,
            UserVMFactory factory)
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
