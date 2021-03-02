using System;
using System.Composition;

#nullable enable

namespace EHunter.UI.Views
{
    internal static class Helpers
    {
        public static object? GetExport(CompositionContext? compositionContext, Type? viewType)
            => viewType is null ? null : compositionContext?.GetExport(viewType);
    }
}
