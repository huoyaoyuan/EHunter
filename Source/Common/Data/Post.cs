using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CA2227

namespace EHunter.Data
{
    public class Post
    {
        public DateTimeOffset PublishedTime { get; set; }
        public DateTimeOffset FavoritedTime { get; set; }

        public IList<ImageEntry> Images { get; set; } = new List<ImageEntry>();

        [Column(TypeName = "varchar(450)")]
        public Uri? Url { get; set; }

        [Column(TypeName = "varchar(32)")]
        public string? Provider { get; set; }
        public int? Identifier { get; set; }
        public string? AdditionalIdentifier { get; set; }

        public string? Title { get; set; }
        public string? DetailText { get; set; }

        public PostGallery? Gallery { get; set; }
    }
}
