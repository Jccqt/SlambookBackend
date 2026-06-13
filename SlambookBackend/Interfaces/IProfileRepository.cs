using SlambookBackend.DTO.Profile;
using SlambookBackend.Models;

namespace SlambookBackend.Interfaces
{
    public interface IProfileRepository
    {
        Task<ServiceResponse<List<MiniProfileDTO>>> GetAllProfiles(int count);
    }
}
