using System.ComponentModel.DataAnnotations;

namespace SlambookBackend.DTO.Users
{
    public class AddUserDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please provide a valid email format.")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
