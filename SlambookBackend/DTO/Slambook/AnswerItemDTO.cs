using System.ComponentModel.DataAnnotations;

namespace SlambookBackend.DTO.Slambook
{
    public class AnswerItemDTO
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        public string AnswerText { get; set; }
    }
}
