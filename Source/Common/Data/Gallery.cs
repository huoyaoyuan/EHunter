using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CA2227

namespace EHunter.Data
{
    public class Gallery
    {
        public string? Name { get; set; }
        public IList<GalleryTag> Tags { get; set; } = new List<GalleryTag>();
    }

    public class PostGallery : Gallery
    {
        private Post? _post;
        public Post Post
        {
            get => _post ?? throw new InvalidOperationException($"Uninitialized property {nameof(Post)}.");
            set => _post = value;
        }
    }

    public class GalleryTag
    {
        public GalleryTag(string? tagScopeName, string tagName)
        {
            TagScopeName = tagScopeName;
            TagName = tagName;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public int GalleryId { get; set; }

        [Column(TypeName = "varchar(32)")]
        public string? TagScopeName { get; set; }
        public string TagName { get; set; }
    }
}
