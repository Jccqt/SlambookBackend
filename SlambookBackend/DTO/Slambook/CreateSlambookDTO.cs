using System.ComponentModel.DataAnnotations;

namespace SlambookBackend.DTO.Slambook
{
    public class CreateSlambookDTO
    {
        [Required]
        public int CreatorId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public List<string> QuestionText { get; set; } 
    }
}
