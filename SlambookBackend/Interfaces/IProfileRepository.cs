using SlambookBackend.DTO.Profile;
using SlambookBackend.Models;

namespace SlambookBackend.Interfaces
{
    public interface IProfileRepository
    {
        Task<ServiceResponse<List<MiniProfileDTO>>> GetAllProfiles(int count);
        Task<ServiceResponse<MiniProfileDTO>> GetProfileByUsername(string username);
        Task<ServiceResponse> UpdateProfile(int userId, string username, string bio, byte[] profilePictureBytes);
    }
}
