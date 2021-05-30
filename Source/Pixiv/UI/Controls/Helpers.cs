using System;
using System.Collections.Generic;
using EHunter.Pixiv.Settings;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
using Microsoft.ApplicationModel.Resources;

namespace EHunter.Pixiv.Controls
{
    internal static class Helpers
    {
        private static readonly Lazy<ResourceLoader> s_resourceLoader = new(() =>
        {
            string? name = typeof(Helpers).Assembly.GetName().Name;
            return new ResourceLoader(name + ".pri", name);
        });

        public static ImageInfo FirstPageMedium(IReadOnlyList<IllustPage> pages)
            => pages[0].Medium;

        public static string LoadFromResources(string key)
            => s_resourceLoader.Value.GetString("Resources/" + key);

        public static string GetIllustRankingMode(IllustRankingMode mode)
            => LoadFromResources($"IllustRankingMode_{mode}");

        public static string GetPixivConnectionMode(PixivConnectionMode mode)
            => LoadFromResources($"PixivConnectionMode_{mode}");
    }
}
