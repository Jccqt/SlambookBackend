using System.ComponentModel.DataAnnotations.Schema;

namespace SlambookBackend.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [Column(TypeName = "mediumblob")]
        public byte[]? ProfilePicture { get; set; }
        public int Status { get; set; }
    }
}
