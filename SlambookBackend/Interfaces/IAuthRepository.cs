using SlambookBackend.DTO.Auth;
using SlambookBackend.Models;

namespace SlambookBackend.Interfaces
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<LoginResponseDTO>> Login(string email, string password);
    }
}
