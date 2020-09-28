using System.Collections.Generic;
using Meowtrix.PixivApi.Models;

namespace EHunter.Provider.Pixiv.Themes
{
    internal partial class IllustSummaryTheme
    {
        public IllustSummaryTheme() => InitializeComponent();

        public static ImageInfo FirstPageMedium(IReadOnlyList<IllustPage> pages) => pages[0].Medium;
    }
}
