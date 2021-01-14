using System.Collections.Generic;
using Meowtrix.PixivApi.Models;

namespace EHunter.Pixiv.Controls
{
    internal static class Helpers
    {
        public static ImageInfo FirstPageMedium(IReadOnlyList<IllustPage> pages)
            => pages[0].Medium;
    }
}
