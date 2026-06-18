using SlambookBackend.DTO.Profile;
using SlambookBackend.Models;

namespace SlambookBackend.Interfaces
{
    public interface IProfileRepository
    {
        Task<ServiceResponse<List<MiniProfileDTO>>> GetAllProfiles(int count, CancellationToken ct = default);
        Task<ServiceResponse<MiniProfileDTO>> GetProfileByUsername(string username, CancellationToken ct = default);
        Task<ServiceResponse<byte[]>> GetProfilePictureBytes(int userId, CancellationToken ct = default);
        Task<ServiceResponse> UpdateProfile(int userId, string firstName, string lastName, string username, string bio, byte[] profilePictureBytes, CancellationToken ct = default);
    }
}
