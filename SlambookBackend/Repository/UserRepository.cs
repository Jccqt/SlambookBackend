using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlambookBackend.Context;
using SlambookBackend.DTO.Users;
using SlambookBackend.Interfaces;
using SlambookBackend.Models;

namespace SlambookBackend.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ServiceResponse<List<UserDTO>>> GetAllUsers()
        {
            var response = new ServiceResponse<List<UserDTO>>();

            var users = await _db.Users
                .AsNoTracking()
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.Username,
                    Bio = u.Bio,
                    ProfilePicture = $"/api/users/{u.Id}/profile-picture"
                }).ToListAsync();

            if(users.Count > 0)
            {
                response.Success = true;
                response.Message = "Users found.";
                response.Data = users;
            }
            else
            {
                response.Message = "No users found.";
            }

            return response;
        }

        public async Task<ServiceResponse<UserDTO>> GetUserById(int userId)
        {
            var response = new ServiceResponse<UserDTO>();

            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if(user != null)
            {
                response.Success = true;
                response.Message = "User found.";
                response.Data = new UserDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Bio = user.Bio,
                    ProfilePicture = $"/api/users/{user.Id}/profile-picture"
                };
            }
            else
            {
                response.Message = "User not found.";
            }

            return response;
        }
    }
}
