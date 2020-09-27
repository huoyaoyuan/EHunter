using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace EHunter.Provider.Pixiv.ViewModels
{
#pragma warning disable CA1001 // 具有可释放字段的类型应该是可释放的
    public class PixivRecentVM : ObservableObject
#pragma warning restore CA1001 // 具有可释放字段的类型应该是可释放的
    {
        private readonly PixivClient _client;
        private readonly ILogger<PixivRecentVM>? _logger;

        public PixivRecentVM(PixivSettings settings, ILogger<PixivRecentVM>? logger = null)
        {
            _client = settings.Client;
            ContinueLogin();

            async void ContinueLogin()
            {
                await settings.InitialLoginTask.ConfigureAwait(true);
                Refresh();
            }

            _logger = logger;
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
                State = RecentPageState.InitialLoading;
                var illusts = Illusts = new ObservableCollection<Illust>();
                var group = GroupedIllusts = new ObservableGroupedCollection<DateTime, Illust>();
                var cts = _lastCts = new CancellationTokenSource();
                try
                {
                    TimeSpan currentOffset = DateTimeOffset.Now.Offset;
                    var source = _client.GetMyFollowingIllustsAsync().Age(SelectedAge);

                    await foreach (var i in source.WithCancellation(cts.Token).ConfigureAwait(true))
                    {
                        State = RecentPageState.PartialLoaded;
                        illusts.Add(i);
                        group.AddItem(i.Created.ToOffset(currentOffset).LocalDateTime.Date, i);
                    }

                    State = RecentPageState.LoadCompleted;
                }
                catch (TaskCanceledException)
                {
                    _logger?.LogInformation("A loading task is cancelled by user option.");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "An error occurred during loading recent updates.");
                }
                finally
                {
                }
            }
            else
            {
                State = RecentPageState.NotLogin;
                Illusts = null;
                GroupedIllusts = null;
            }
        }

        private RecentPageState _state = RecentPageState.WaitingLogin;
        public RecentPageState State
        {
            get => _state;
            private set => SetProperty(ref _state, value);
        }

        // TODO: https://github.com/microsoft/microsoft-ui-xaml/issues/3339

        private AgeRestriction _selectedAge = AgeRestriction.AllAge;
        public AgeRestriction SelectedAge
        {
            get => _selectedAge;
            set
            {
                if (SetProperty(ref _selectedAge, value))
                {
                    OnPropertyChanged(nameof(IntSelectedAge));
                    Refresh();
                }
            }
        }

        public int IntSelectedAge
        {
            get => (int)SelectedAge;
            set => SelectedAge = (AgeRestriction)value;
        }

        private CancellationTokenSource? _lastCts;

        private ObservableCollection<Illust>? _illusts;
        public ObservableCollection<Illust>? Illusts
        {
            get => _illusts;
            private set => SetProperty(ref _illusts, value);
        }

        private ObservableGroupedCollection<DateTime, Illust>? _groupedIllusts;
        public ObservableGroupedCollection<DateTime, Illust>? GroupedIllusts
        {
            get => _groupedIllusts;
            private set => SetProperty(ref _groupedIllusts, value);
        }
    }

    public enum RecentPageState
    {
        WaitingLogin,
        NotLogin,
        InitialLoading,
        PartialLoaded,
        LoadCompleted
    }
}
