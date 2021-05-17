using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using EHunter.Pixiv.Settings;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.Resources
{
    // TODO: https://github.com/microsoft/microsoft-ui-xaml/issues/3339

    internal class EnumValueCollection<T> : Collection<int>
        where T : struct, Enum
    {
        public EnumValueCollection()
        {
            foreach (T value in Enum.GetValues(typeof(T)))
                Add(Unsafe.As<T, int>(ref Unsafe.AsRef(in value)));
        }
    }

    internal class AgeRestrictionValues : Collection<AgeRestriction>
    {
        public AgeRestrictionValues()
        {
            foreach (AgeRestriction value in Enum.GetValues(typeof(AgeRestriction)))
                Add(value);
        }
    }

    internal class IllustSearchTargetValues : EnumValueCollection<IllustSearchTarget>
    {
    }

    internal class IllustSortModeValues : EnumValueCollection<IllustSortMode>
    {
    }

    internal class IllustRankingModeCollection : EnumValueCollection<IllustRankingMode>
    {
    }

    internal class PixivConnectionModeCollection : EnumValueCollection<PixivConnectionMode>
    {
    }
}
