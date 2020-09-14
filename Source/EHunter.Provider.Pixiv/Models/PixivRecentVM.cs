using Meowtrix.PixivApi;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.Provider.Pixiv.Models
{
    public class PixivRecentVM : ObservableObject
    {
        private readonly PixivClient _client;

        public PixivRecentVM(PixivSettings settings)
        {
            _client = settings.Client;
            ContinueLogin();

            async void ContinueLogin()
            {
                await settings.InitialLoginTask.ConfigureAwait(true);
                Refresh();
            }
        }

        public void Refresh()
        {
            if (_client.IsLogin)
            {
                NotLogin = false;
            }
            else
            {
                NotLogin = true;
            }
        }

        private bool _notLogin = true;
        public bool NotLogin
        {
            get => _notLogin;
            private set => SetProperty(ref _notLogin, value);
        }
    }
}
