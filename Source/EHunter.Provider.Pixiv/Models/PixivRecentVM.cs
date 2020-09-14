using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace EHunter.Provider.Pixiv.Models
{
#pragma warning disable CA1001 // 具有可释放字段的类型应该是可释放的
    public class PixivRecentVM : ObservableObject
#pragma warning restore CA1001 // 具有可释放字段的类型应该是可释放的
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

        public async void Refresh()
        {
            if (_lastCts != null)
            {
                _lastCts.Cancel();
                _lastCts.Dispose();
                _lastCts = null;
            }

            if (_client.IsLogin)
            {
                NotLogin = false;
                var illusts = Illusts = new ObservableCollection<Illust>();
                var cts = _lastCts = new CancellationTokenSource();
                try
                {
                    var source = _client.GetMyFollowingIllustsAsync();
                    source = SelectedModeIndex switch
                    {
                        1 => source.AllAge(),
                        2 => source.R18(),
                        _ => source
                    };

                    await foreach (var i in source.WithCancellation(cts.Token).ConfigureAwait(true))
                        illusts.Add(i);
                }
                catch
                {
                }
            }
            else
            {
                NotLogin = true;
                Illusts = null;
            }
        }

        private bool _notLogin = true;
        public bool NotLogin
        {
            get => _notLogin;
            private set => SetProperty(ref _notLogin, value);
        }

        private int _selectedModeIndex = 1;
        public int SelectedModeIndex
        {
            get => _selectedModeIndex;
            set
            {
                if (SetProperty(ref _selectedModeIndex, value))
                    Refresh();
            }
        }

        private CancellationTokenSource? _lastCts;

        private ObservableCollection<Illust>? _illusts;
        public ObservableCollection<Illust>? Illusts
        {
            get => _illusts;
            private set => SetProperty(ref _illusts, value);
        }
    }
}
