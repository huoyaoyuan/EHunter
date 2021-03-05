using System;

#nullable enable

namespace EHunter.UI.ViewModels
{
    public abstract class NavigationEntry
    {
        public string? Title { get; set; }
        public Type? ViewType { get; set; }
    }

    public class IconNavigationEntry : NavigationEntry
    {
        public Uri? IconUri { get; set; }
    }

    public class GlyphNavigationEntry : NavigationEntry
    {
        public string? Glyph { get; set; }
    }
}
