using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EHunter.Pixiv.Data
{
    public class PendingDownload
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ArtworkId { get; set; }
        public DateTimeOffset Time { get; set; }
        [ConcurrencyCheck]
        public int PId { get; set; }
    }
}
