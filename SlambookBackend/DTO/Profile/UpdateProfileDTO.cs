using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SlambookBackend.DTO.Profile
{
    public class UpdateProfileDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Bio { get; set; }

        [Required]
        public IFormFile ProfilePicture { get; set; }
    }
}
