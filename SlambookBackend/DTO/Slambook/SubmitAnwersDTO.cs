using System.ComponentModel.DataAnnotations;

namespace SlambookBackend.DTO.Slambook
{
    public class SubmitAnwersDTO
    {
        [Required]
        public int ResponderId { get; set; }

        [Required]
        public List<AnswerItemDTO> Answers { get; set; } = new List<AnswerItemDTO>(); 
    }
}
