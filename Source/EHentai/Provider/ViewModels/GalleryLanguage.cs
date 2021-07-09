using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            .ToDictionary(x => x.EnglishName, StringComparer.OrdinalIgnoreCase);
    }
}
