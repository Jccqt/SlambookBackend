using System.ComponentModel.DataAnnotations.Schema;

namespace SlambookBackend.Models
{
    public class Questions
    {
        public int Id { get; set; }

        [Column("slambook_id")]
        public int SlambookId { get; set; }

        [Column("question_text")]
        public string QuestionText { get; set; } = string.Empty;
    }
}
