using System.Collections.Immutable;

namespace EHunter.EHentai.Api.Models
{
    public struct GalleryListPage
    {
        public GalleryListPage(int totalCount, int pagesCount, ImmutableArray<Gallery> galleries)
        {
            TotalCount = totalCount;
            PagesCount = pagesCount;
            Galleries = galleries;
        }

        public int TotalCount { get; }
        public int PagesCount { get; }
        public ImmutableArray<Gallery> Galleries { get; }
    }
}
