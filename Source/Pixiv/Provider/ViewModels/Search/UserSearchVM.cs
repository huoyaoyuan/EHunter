using System.Composition;
using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.Services;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.Pixiv.ViewModels.Search
{
    [Export]
    public class UserSearchVM : ObservableObject
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

        private string _searchWord = string.Empty;
        public string SearchWord
        {
            get => _searchWord;
            set => SetProperty(ref _searchWord, value);
        }

        private IBindableCollection<UserInfoWithPreview>? _users;
        public IBindableCollection<UserInfoWithPreview>? Users
        {
            get => _users;
            private set => SetProperty(ref _users, value);
        }

        public void Search()
        {
            var client = _clientResolver.Resolve();
            var users = client.SearchUsersAsync(SearchWord);
            Users = _viewModelService.CreateAsyncCollection(users);
        }
    }
}
