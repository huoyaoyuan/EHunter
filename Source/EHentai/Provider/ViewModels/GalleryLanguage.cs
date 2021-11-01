using System.Globalization;
using EHunter.EHentai.Api.Models;

namespace EHunter.EHentai.ViewModels
{
    public readonly struct GalleryLanguage
    {
        private static readonly Tag s_translatedTag = new("language", "translated");
        private static readonly Tag s_rewriteTag = new("language", "rewrite");

        public GalleryLanguage(IEnumerable<Tag> languageTags)
        {
            IsTranslated = languageTags.Contains(s_translatedTag);
            IsRewritten = languageTags.Contains(s_rewriteTag);

            Languages = languageTags
                .Where(x => x is ("language", not "translated" or "rewrite"))
                .Select(x => s_languageTable.TryGetValue(x.Name, out var culture)
                    ? culture.DisplayName : x.Name)
                .ToArray();
        }

        public bool IsTranslated { get; }
        public bool IsRewritten { get; }
        public bool IsMultiple => Languages.Count > 1;

        public IReadOnlyList<string> Languages { get; }

        private static readonly Dictionary<string, CultureInfo> s_languageTable
            = CultureInfo.GetCultures(CultureTypes.NeutralCultures)
            .Distinct(new EnglishNameComparer())
            .ToDictionary(x => x.EnglishName, StringComparer.OrdinalIgnoreCase);

        // TODO: use DistinctBy
        private sealed class EnglishNameComparer : IEqualityComparer<CultureInfo>
        {
            public bool Equals(CultureInfo? x, CultureInfo? y) => string.Equals(x?.EnglishName, y?.EnglishName, StringComparison.OrdinalIgnoreCase);
            public int GetHashCode(CultureInfo obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.EnglishName);
        }
    }
}
