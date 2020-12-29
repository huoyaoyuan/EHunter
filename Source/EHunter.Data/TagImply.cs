using System.ComponentModel.DataAnnotations.Schema;

namespace EHunter.Data
{
    public class TagImply
    {
        public TagImply(string? tagScopeName, string tagName, string? impliedTagScopeName, string impliedTagName)
        {
            TagScopeName = tagScopeName;
            TagName = tagName;
            ImpliedTagScopeName = impliedTagScopeName;
            ImpliedTagName = impliedTagName;
        }

        [Column(TypeName = "varchar(32)")]
        public string? TagScopeName { get; set; }
        public string TagName { get; set; }

        [Column(TypeName = "varchar(32)")]
        public string? ImpliedTagScopeName { get; set; }
        public string ImpliedTagName { get; set; }
    }
}
