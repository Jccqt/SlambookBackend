using System.ComponentModel.DataAnnotations;

namespace SlambookBackend.DTO.Users
{
    public class UpdatePasswordDTO
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
