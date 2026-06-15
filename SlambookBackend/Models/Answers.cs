using System.ComponentModel.DataAnnotations.Schema;

namespace SlambookBackend.Models
{
    public class Answers
    {
        public int Id { get; set; }

        [Column("question_id")]
        public int QuestionId { get; set; }

        [Column("responder_id")]
        public int ResponderId { get; set; }

        [Column("answer_text")]
        public string AnswerText { get; set; } = string.Empty;

        public Questions? Question { get; set; }
    }
}
