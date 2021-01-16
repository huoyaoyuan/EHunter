﻿using System;
using System.Collections.Generic;
using EHunter.DependencyInjection;
using EHunter.Pixiv.ViewModels.Primitives;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.ViewModels.Ranking
{
    public class RankingVM : IllustCollectionVM
    {
        private readonly ICustomResolver<PixivClient> _clientResolver;

        public RankingVM(ICustomResolver<PixivClient> clientResolver,
            IllustVMFactory illustVMFactory)
            : base(illustVMFactory)
        {
            _clientResolver = clientResolver;
            Refresh();
        }

        protected override IAsyncEnumerable<Illust>? LoadIllusts()
            => _clientResolver.Resolve().GetIllustRankingAsync(SelectedRankingMode, Date.Date);

        public int IntSelectedRankingMode
        {
            get => (int)SelectedRankingMode;
            set => SelectedRankingMode = (IllustRankingMode)value;
        }

        private IllustRankingMode _selectedRaningMode;
        public IllustRankingMode SelectedRankingMode
        {
            get => _selectedRaningMode;
            set
            {
                if (SetProperty(ref _selectedRaningMode, value))
                {
                    OnPropertyChanged(nameof(IntSelectedRankingMode));
                    Refresh();
                }
            }
        }

        private DateTimeOffset _date = DateTimeOffset.Now;
        public DateTimeOffset Date
        {
            get => _date;
            set
            {
                if (SetProperty(ref _date, value))
                    Refresh();
            }
        }
    }
}
