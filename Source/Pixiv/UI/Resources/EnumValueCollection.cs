using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
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

    internal class AgeRestrictionValues : EnumValueCollection<AgeRestriction>
    {
    }

    internal class IllustSearchTargetValues : EnumValueCollection<IllustSearchTarget>
    {
    }

    internal class IllustSortModeValues : EnumValueCollection<IllustSortMode>
    {
    }
}
