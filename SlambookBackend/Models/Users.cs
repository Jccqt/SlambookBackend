using System.ComponentModel.DataAnnotations.Schema;

namespace SlambookBackend.Models
{
    public class Users
    {
        public int Id { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [Column("bio")]
        public string Bio { get; set; } = string.Empty;

        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Column("password")]
        public string Password { get; set; } = string.Empty;

        [Column("salt")]
        public string Salt { get; set; } = string.Empty;

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("profile_picture", TypeName = "mediumblob")]
        public byte[]? ProfilePicture { get; set; }
        public int Status { get; set; }

        [Column("login_count")]
        public int LoginCount { get; set; }

        public ICollection<Slambooks> Slambooks { get; set; }
    }
}
