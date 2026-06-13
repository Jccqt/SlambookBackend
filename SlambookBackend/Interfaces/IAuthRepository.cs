using SlambookBackend.Models;

namespace SlambookBackend.Interfaces
{
    public interface IAuthRepository
    {
        Task<ServiceResponse> Login(string email, string password);
    }
}
