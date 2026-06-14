using System.ComponentModel.DataAnnotations.Schema;

namespace SlambookBackend.Models
{
    public class Slambooks
    {
        public int Id { get; set; }

        [Column("creator_id")]
        public int CreatorId { get; set; }

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("date_created")]
        public DateOnly CreatedDate { get; set; }

        public Users? Creator { get; set; }
    }
}
