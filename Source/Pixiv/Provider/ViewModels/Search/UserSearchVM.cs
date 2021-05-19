using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.Services;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Search
{
    [Export]
    [ObservableProperty("SearchWord", typeof(string), Initializer = "string.Empty")]
    [ObservableProperty("Users", typeof(IBindableCollection<UserInfoWithPreview>), IsNullable = true, IsSetterPublic = false)]
    public partial class UserSearchVM : ObservableObject
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;
        private readonly IViewModelService _viewModelService;

        [ImportingConstructor]
        public UserSearchVM(ICustomResolver<PixivClient> clientResolver,
            IViewModelService viewModelService)
        {
            _clientResolver = clientResolver;
            _viewModelService = viewModelService;
        }

        public void Search()
        {
            var client = _clientResolver.Resolve();
            var users = client.SearchUsersAsync(SearchWord);
            Users = _viewModelService.CreateAsyncCollection(users);
        }
    }
}
