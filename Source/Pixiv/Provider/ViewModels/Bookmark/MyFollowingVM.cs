using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.Services;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Bookmark
{
    [Export]
    public class MyFollowingVM : ObservableObject
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;
        private readonly IViewModelService _viewModelService;

        [ImportingConstructor]
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
