using SlambookBackend.DTO.Users;
using SlambookBackend.Models;

namespace SlambookBackend.Interfaces
{
    public interface IUserRepository
    {
        Task<ServiceResponse<List<UserDTO>>> GetAllUsers(CancellationToken ct = default);
        Task<ServiceResponse<UserDTO>> GetUserById(int userId, CancellationToken ct = default);
        Task<ServiceResponse<string>> GetUsernameById(int userId, CancellationToken ct = default);
        Task<ServiceResponse> AddUser(AddUserDTO user, CancellationToken ct = default);
        Task<ServiceResponse> UpdateLoginCount(int userId, CancellationToken ct = default);
        Task<ServiceResponse> UpdatePassword(int userId, UpdatePasswordDTO password, CancellationToken ct = default);
    }
}
