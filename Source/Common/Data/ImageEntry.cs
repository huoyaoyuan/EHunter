using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CA2227

namespace EHunter.Data
{
    public class ImageEntry
    {
        public ImageEntry(ImageType type, string storagePath)
        {
            Type = type;
            StoragePath = storagePath;
        }

        public ImageType Type { get; set; }
        public string StoragePath { get; set; }

        [Column(TypeName = "varchar(450)")]
        public Uri? Url { get; set; }

        private Post? _post;
        public Post Post
        {
            get => _post ?? throw new InvalidOperationException($"Uninitialized property {nameof(Post)}.");
            set => _post = value;
        }

        public int PostId { get; private set; }
        public int PostOrderId { get; set; }

        public IList<ImageTag> Tags { get; set; } = new List<ImageTag>();
    }

    public class ImageTag
    {
        public ImageTag(string? tagScopeName, string tagName)
        {
            TagScopeName = tagScopeName;
            TagName = tagName;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public int ImageId { get; set; }

        [Column(TypeName = "varchar(32)")]
        public string? TagScopeName { get; set; }
        public string TagName { get; set; }
    }

    public enum ImageType
    {
        Static = 0,
        Animated = 1,
        Video = 2,
    }
}
