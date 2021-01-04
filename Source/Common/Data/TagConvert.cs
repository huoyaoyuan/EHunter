using System.ComponentModel.DataAnnotations.Schema;

namespace EHunter.Data
{
    public class TagConvert
    {
        public TagConvert(string? tagScopeName, string tagName, string? convertedTagScopeName, string convertedTagName)
        {
            TagScopeName = tagScopeName;
            TagName = tagName;
            ConvertedTagScopeName = convertedTagScopeName;
            ConvertedTagName = convertedTagName;
        }

        [Column(TypeName = "varchar(32)")]
        public string? TagScopeName { get; set; }
        public string TagName { get; set; }

        [Column(TypeName = "varchar(32)")]
        public string? ConvertedTagScopeName { get; set; }
        public string ConvertedTagName { get; set; }
    }
}
