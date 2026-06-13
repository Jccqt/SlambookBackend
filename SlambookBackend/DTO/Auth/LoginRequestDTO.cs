using System.ComponentModel.DataAnnotations;

namespace SlambookBackend.DTO.Auth
{
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress(ErrorMessage = "Please provide a valid email format.")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
