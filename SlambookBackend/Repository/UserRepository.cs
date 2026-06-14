using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlambookBackend.Context;
using SlambookBackend.DTO.Users;
using SlambookBackend.Interfaces;
using SlambookBackend.Models;
using SlambookBackend.Tools;

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

        public async Task<ServiceResponse> AddUser(AddUserDTO user)
        {
            var response = new ServiceResponse();

            bool isExists = await _db.Users
                .AnyAsync(u => u.Email == user.Email);

            if (isExists)
            {
                response.Message = "Account already exists.";
                return response;
            }

            string salt = Crypt.GenerateSalt();
            string hashedPassword = Crypt.HashPassword(user.Password, salt);

            var newUser = new Users
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = hashedPassword,
                Salt = salt,
                LoginCount = 0,
                Status = 1
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            response.Success = true;
            response.Message = "Account created successfully.";

            return response;
        }

        public async Task<ServiceResponse> UpdateLoginCount(int userId)
        {
            var response = new ServiceResponse();

            int affectedRows = await _db.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(u => u.LoginCount, u => u.LoginCount + 1));

            if (affectedRows == 0)
            {
                response.Message = "Failed to update login count. User not found.";
            }
            else
            {
                response.Success = true;
                response.Message = "Successfully updated login count.";
            }

            return response;
        }
    }
}
