using SlambookBackend.DTO.Slambook;
using SlambookBackend.Models;

namespace SlambookBackend.Interfaces
{
    public interface ISlambookRepository
    {
        Task<ServiceResponse<List<SlambookDTO>>> GetAllSlambooks(int count);
        Task<ServiceResponse<SlambookDetailsDTO>> GetSlambookDetails(int slambookId);
    }
}
