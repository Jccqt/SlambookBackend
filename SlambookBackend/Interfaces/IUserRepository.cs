using SlambookBackend.DTO.Users;
using SlambookBackend.Models;

namespace SlambookBackend.Interfaces
{
    public interface IUserRepository
    {
        Task<ServiceResponse<List<UserDTO>>> GetAllUsers();
        Task<ServiceResponse<UserDTO>> GetUserById(int userId);
        Task<ServiceResponse<string>> GetUsernameById(int userId);
        Task<ServiceResponse> AddUser(AddUserDTO user);
        Task<ServiceResponse> UpdateLoginCount(int userId);
        Task<ServiceResponse> UpdatePassword(int userId, UpdatePasswordDTO password);
    }
}
