using System.Collections.Immutable;

namespace EHunter.EHentai.Api.Models
{
    public struct GalleryListPage
    {
        public GalleryListPage(int totalCount, ImmutableArray<Gallery> galleries)
        {
            TotalCount = totalCount;
            Galleries = galleries;
        }

        public int TotalCount { get; }
        public ImmutableArray<Gallery> Galleries { get; }
    }
}
