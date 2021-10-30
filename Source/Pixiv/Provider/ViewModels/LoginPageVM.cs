﻿using System;
using System.Composition;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using EHunter.Data;
using EHunter.Pixiv.Data;
using EHunter.Pixiv.Messages;
using EHunter.Pixiv.Services;
using EHunter.Pixiv.Settings;

namespace EHunter.Pixiv.ViewModels
{
    [Export]
    [ObservableProperty("ConnectionMode", typeof(PixivConnectionMode), ChangedAction = "_setting.SetConnectionOption(value);")]
    public partial class LoginPageVM : ObservableObject
    {
        private readonly IPixivSettingStore _settingStore;
        private readonly PixivSetting _setting;
        private readonly PixivClientService _clientService;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private string _refreshToken = string.Empty;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private bool _isLoggingIn;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private bool _isLoggedin;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private bool? _databaseInitState;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private bool _showLoginException;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private Exception? _loginException;

        [ImportingConstructor]
        public LoginPageVM(
            IPixivSettingStore settingStore,
            PixivSetting setting,
            PixivClientService clientService,
            EHunterDbContextResolver<EHunterDbContext> eHunterContextResolver,
            EHunterDbContextResolver<PixivDbContext> pixivContextResolver)
        {
            _settingStore = settingStore;
            _setting = setting;
            _clientService = clientService;

            _connectionMode = _setting.ConnectionMode.Value;

            string? savedToken = settingStore.RefreshToken;
            if (!string.IsNullOrEmpty(savedToken))
            {
                RefreshToken = savedToken;
                LoginWithToken();
            }

            InitDatabase();
            async void InitDatabase()
            {
                await Task.WhenAll(eHunterContextResolver.InitializeTask, pixivContextResolver.InitializeTask).ConfigureAwait(true);
                DatabaseInitState = eHunterContextResolver.Resolve() is not null && pixivContextResolver.Resolve() is not null;
                CheckInitialize();
            }
        }

        public void LoginWithWebView(Func<string, Task<Uri>> browserTask) => PerformLogin(_clientService.LoginAsync(browserTask));

        public void LoginWithToken() => PerformLogin(_clientService.LoginAsync(RefreshToken));

        private async void PerformLogin(Task<string> loginTask)
        {
            IsLoggingIn = true;
            ShowLoginException = false;
            LoginException = null;

            try
            {
                string refreshToken = await loginTask.ConfigureAwait(true);
                _settingStore.RefreshToken = refreshToken;
                IsLoggedin = true;
            }
            catch (Exception ex)
            {
                LoginException = ex;
                ShowLoginException = true;
            }

            IsLoggingIn = false;
            CheckInitialize();
        }

        private void CheckInitialize()
        {
            if (IsLoggedin && DatabaseInitState != null)
            {
                WeakReferenceMessenger.Default.Send(new InitializationCompleteMessage());
            }
        }
    }
}
