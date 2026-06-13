namespace SlambookBackend.DTO.Profile
{
    public class MiniProfileDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
    }
}
