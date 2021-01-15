using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.Services;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.Pixiv.ViewModels.Bookmark
{
    public class MyFollowingVM : ObservableObject
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;
        private readonly IViewModelService _viewModelService;

        public MyFollowingVM(ICustomResolver<PixivClient> clientResolver,
            IViewModelService viewModelService)
        {
            _clientResolver = clientResolver;
            _viewModelService = viewModelService;

            Refresh();
        }

        public void Refresh()
        {
            var client = _clientResolver.Resolve();
            Users = _viewModelService.CreateAsyncCollection(client.GetMyFollowingUsersAsync());
        }

        private IBindableCollection<UserInfoWithPreview>? _users;
        public IBindableCollection<UserInfoWithPreview>? Users
        {
            get => _users;
            private set => SetProperty(ref _users, value);
        }
    }
}
