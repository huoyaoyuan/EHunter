using System.Collections.ObjectModel;
using EHunter.Pixiv.Settings;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.Resources
{
    internal class EnumValueCollection<T> : Collection<T>
        where T : struct, Enum
    {
        public EnumValueCollection()
        {
            foreach (T value in Enum.GetValues<T>())
                Add(value);
        }
    }

    internal class AgeRestrictionValues : EnumValueCollection<AgeRestriction>
    {
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
