using EHunter.Pixiv.Settings;
using Meowtrix.PixivApi;
using Microsoft.Windows.ApplicationModel.Resources;

namespace EHunter.Pixiv.Controls
{
    internal static class Helpers
    {
        private static readonly Lazy<ResourceLoader> s_resourceLoader = new(() =>
        {
            string? name = typeof(Helpers).Assembly.GetName().Name;
            return new ResourceLoader(name + ".pri", name);
        });

        public static string LoadFromResources(string key)
            => s_resourceLoader.Value.GetString("Resources/" + key);

        public static string GetIllustRankingMode(IllustRankingMode mode)
            => LoadFromResources($"IllustRankingMode_{mode}");

        public static string GetPixivConnectionMode(PixivConnectionMode mode)
            => LoadFromResources($"PixivConnectionMode_{mode}");
    }
}
