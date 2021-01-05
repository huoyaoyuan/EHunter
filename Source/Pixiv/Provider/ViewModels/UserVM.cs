using System;
using EHunter.ComponentModel;
using EHunter.DependencyInjection;
using EHunter.Services;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.Pixiv.ViewModels
{
    public class UserVM : ObservableObject
    {
        private readonly PixivClient _client;
        private readonly IViewModelService _viewModelService;
        private readonly IllustVMFactory _illustVMFactory;
        private int _userId;
        public int UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        public async void JumpToUser()
        {
            try
            {
                IsLoading = true;

                var user = await _client.GetUserDetailAsync(UserId).ConfigureAwait(true);
                UserInfo = user;
                UserDetail = user;
                LoadIllusts();
            }
            catch
            {
            }
            finally
            {
                IsLoading = false;
            }
        }

        public UserVM(PixivClient client, IViewModelService viewModelService, IllustVMFactory illustVMFactory)
        {
            _client = client;
            _viewModelService = viewModelService;
            _illustVMFactory = illustVMFactory;
        }

        public UserVM(UserInfo userInfo, PixivClient client, IViewModelService viewModelService, IllustVMFactory illustVMFactory)
        {
            _client = client;
            _viewModelService = viewModelService;
            _illustVMFactory = illustVMFactory;
            UserInfo = userInfo;
            Load(userInfo);

            async void Load(UserInfo user)
            {
                LoadIllusts();
                UserDetail = await user.GetDetailAsync().ConfigureAwait(true);
            }
        }

        private void LoadIllusts()
        {
            var illusts = UserInfo?.GetIllustsAsync().Age(SelectedAge);
            var vms = _illustVMFactory.CreateViewModels(illusts);
            Illusts = _viewModelService.CreateAsyncCollection(vms);

            // TODO: Consider AdvancedCollectionView.Filter
            // Currently doesn't work with mignon/IsR18=true
            // 8.0.0-preview2
        }

        private UserInfo? _userInfo;
        public UserInfo? UserInfo
        {
            get => _userInfo;
            private set => SetProperty(ref _userInfo, value);
        }

        private UserDetailInfo? _userDetail;
        public UserDetailInfo? UserDetail
        {
            get => _userDetail;
            private set => SetProperty(ref _userDetail, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        private AgeRestriction _selectedAge = AgeRestriction.All;
        public AgeRestriction SelectedAge
        {
            get => _selectedAge;
            set
            {
                if (SetProperty(ref _selectedAge, value))
                {
                    OnPropertyChanged(nameof(IntSelectedAge));
                    LoadIllusts();
                }
            }
        }

        public int IntSelectedAge
        {
            get => (int)SelectedAge;
            set => SelectedAge = (AgeRestriction)value;
        }

        private IBindableCollection<IllustVM>? _illusts;
        public IBindableCollection<IllustVM>? Illusts
        {
            get => _illusts;
            private set => SetProperty(ref _illusts, value);
        }

        public Uri? Url => UserInfo is null ? null
            : new($"https://www.pixiv.net/users/{UserInfo.Id}");
    }

    public class UserVMFactory
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;
        private readonly IViewModelService _viewModelService;
        private readonly IllustVMFactory _illustVMFactory;

        public UserVMFactory(ICustomResolver<PixivClient> clientResolver,
            IViewModelService viewModelService,
            IllustVMFactory illustVMFactory)
        {
            _clientResolver = clientResolver;
            _viewModelService = viewModelService;
            _illustVMFactory = illustVMFactory;
        }

        public UserVM Create() => new(_clientResolver.Resolve(), _viewModelService, _illustVMFactory);
        public UserVM Create(UserInfo userInfo) => new(userInfo, _clientResolver.Resolve(), _viewModelService, _illustVMFactory);
    }
}
