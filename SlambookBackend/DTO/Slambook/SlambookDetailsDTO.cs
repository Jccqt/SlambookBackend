using SlambookBackend.DTO.Profile;

namespace SlambookBackend.DTO.Slambook
{
    public class SlambookDetailsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly CreatedDate { get; set; }

        public ICollection<MiniProfileDTO> Responses = new List<MiniProfileDTO>();
    }
}
