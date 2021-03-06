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
    [ObservableProperty("RefreshToken", typeof(string), Initializer = "string.Empty")]
    [ObservableProperty("ConnectionMode", typeof(PixivConnectionMode), ChangedAction = "_setting.SetConnectionOption(value);")]
    [ObservableProperty("IsLoggingIn", typeof(bool), IsSetterPublic = false)]
    [ObservableProperty("IsLoggedin", typeof(bool), IsSetterPublic = false)]
    [ObservableProperty("DatabaseInitState", typeof(bool?))]
    [ObservableProperty("ShowLoginException", typeof(bool))]
    [ObservableProperty("LoginException", typeof(Exception), IsNullable = true)]
    public partial class LoginPageVM : ObservableObject
    {
        private readonly IPixivSettingStore _settingStore;
        private readonly PixivSetting _setting;
        private readonly PixivClientService _clientService;

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
